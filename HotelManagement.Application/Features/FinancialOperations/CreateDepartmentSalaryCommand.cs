using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.FinancialOperations;

public record CreateDepartmentSalaryCommand(
    long DepartmentId,
    long CreatedById,
    bool IsSalary) : IRequest<int>; 

public class CreateDepartmentSalaryCommandHandler : IRequestHandler<CreateDepartmentSalaryCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateDepartmentSalaryCommandHandler> _logger;

    public CreateDepartmentSalaryCommandHandler(IApplicationDbContext context, ILogger<CreateDepartmentSalaryCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> Handle(CreateDepartmentSalaryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Выдача {Type} для отдела ID: {DepartmentId}",
            request.IsSalary ? "зарплаты" : "премии", request.DepartmentId);

        var employees = await _context.Employees
            .Include(e => e.User)
            .Where(e => e.DepartmentId == request.DepartmentId && e.IsActive)
            .ToListAsync(cancellationToken);

        if (!employees.Any())
        {
            throw new Exception("В отделе нет активных сотрудников");
        }

        var operations = new List<FinancialOperation>();

        foreach (var employee in employees)
        {
            int amount;
            string description;

            if (request.IsSalary)
            {
                amount = (employee.Salary ?? 0) + (employee.SalarySupplement ?? 0);
                description = $"Зарплата за месяц";
            }
            else
            {
                amount = (employee.SalarySupplement ?? 0);
                description = $"Премия";
            }

            operations.Add(new FinancialOperation
            {
                EmployeeId = employee.Id,
                Amount = amount,
                Description = description,
                CreatedById = request.CreatedById,
                CreatedAt = DateTimeOffset.UtcNow
            });
        }

        _context.FinancialOperations.AddRange(operations);
        await _context.SaveChangesAsync(cancellationToken);

        return operations.Count;
    }
}