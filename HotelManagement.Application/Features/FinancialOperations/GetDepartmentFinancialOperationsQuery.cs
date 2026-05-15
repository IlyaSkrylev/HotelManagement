using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.FinancialOperations;

public record GetDepartmentFinancialOperationsQuery(
    long DepartmentId,
    int Page = 1,
    int PageSize = 20) : IRequest<PaginatedResult<FinancialOperationWithEmployeeDto>>;

public class GetDepartmentFinancialOperationsQueryHandler : IRequestHandler<GetDepartmentFinancialOperationsQuery, PaginatedResult<FinancialOperationWithEmployeeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetDepartmentFinancialOperationsQueryHandler> _logger;

    public GetDepartmentFinancialOperationsQueryHandler(IApplicationDbContext context, ILogger<GetDepartmentFinancialOperationsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResult<FinancialOperationWithEmployeeDto>> Handle(GetDepartmentFinancialOperationsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос финансовых операций для отдела ID: {DepartmentId}", request.DepartmentId);

        var query = _context.FinancialOperations
            .Include(fo => fo.Employee)
            .ThenInclude(e => e.User)
            .Where(fo => fo.Employee.DepartmentId == request.DepartmentId)
            .OrderByDescending(fo => fo.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(fo => new FinancialOperationWithEmployeeDto
            {
                Id = fo.Id,
                EmployeeId = fo.EmployeeId,
                EmployeeName = fo.Employee.User.LastName + " " + fo.Employee.User.FirstName,
                Amount = fo.Amount,
                Description = fo.Description,
                CreatedById = fo.CreatedById,
                CreatedAt = fo.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<FinancialOperationWithEmployeeDto>(items, totalCount, request.Page, request.PageSize);
    }
}