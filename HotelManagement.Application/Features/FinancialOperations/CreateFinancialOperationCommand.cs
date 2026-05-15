using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.FinancialOperations;

public record CreateFinancialOperationCommand(
    long EmployeeId,
    int Amount,
    string? Description,
    long CreatedById) : IRequest<FinancialOperationDto>;

public class CreateFinancialOperationCommandHandler : IRequestHandler<CreateFinancialOperationCommand, FinancialOperationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateFinancialOperationCommandHandler> _logger;

    public CreateFinancialOperationCommandHandler(IApplicationDbContext context, ILogger<CreateFinancialOperationCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<FinancialOperationDto> Handle(CreateFinancialOperationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Создание финансовой операции для сотрудника ID: {EmployeeId}, Сумма: {Amount}", request.EmployeeId, request.Amount);

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId && e.IsActive, cancellationToken);

        if (employee == null)
        {
            throw new Exception("Сотрудник не найден");
        }

        var operation = new FinancialOperation
        {
            EmployeeId = request.EmployeeId,
            Amount = request.Amount,
            Description = request.Description,
            CreatedById = request.CreatedById,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.FinancialOperations.Add(operation);
        await _context.SaveChangesAsync(cancellationToken);

        return new FinancialOperationDto
        {
            Id = operation.Id,
            EmployeeId = operation.EmployeeId,
            Amount = operation.Amount,
            Description = operation.Description,
            CreatedById = operation.CreatedById,
            CreatedAt = operation.CreatedAt
        };
    }
}