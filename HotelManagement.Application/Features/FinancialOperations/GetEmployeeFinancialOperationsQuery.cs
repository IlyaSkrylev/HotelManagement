using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.FinancialOperations;

public record GetEmployeeFinancialOperationsQuery(
    long EmployeeId,
    int Page = 1,
    int PageSize = 20,
    string? StartDate = null,
    string? EndDate = null) : IRequest<PaginatedResult<FinancialOperationDto>>;

public class GetEmployeeFinancialOperationsQueryHandler : IRequestHandler<GetEmployeeFinancialOperationsQuery, PaginatedResult<FinancialOperationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetEmployeeFinancialOperationsQueryHandler> _logger;

    public GetEmployeeFinancialOperationsQueryHandler(IApplicationDbContext context, ILogger<GetEmployeeFinancialOperationsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResult<FinancialOperationDto>> Handle(GetEmployeeFinancialOperationsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос финансовых операций для сотрудника ID: {EmployeeId}", request.EmployeeId);

        var query = _context.FinancialOperations
            .Where(fo => fo.EmployeeId == request.EmployeeId);

        if (!string.IsNullOrWhiteSpace(request.StartDate))
        {
            if (DateTime.TryParse(request.StartDate, out var startDate))
            {
                var startDateUtc = new DateTimeOffset(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, TimeSpan.Zero);
                query = query.Where(fo => fo.CreatedAt >= startDateUtc);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.EndDate))
        {
            if (DateTime.TryParse(request.EndDate, out var endDate))
            {
                var endDateUtc = new DateTimeOffset(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59, TimeSpan.Zero);
                query = query.Where(fo => fo.CreatedAt <= endDateUtc);
            }
        }

        query = query.OrderByDescending(fo => fo.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(fo => new FinancialOperationDto
            {
                Id = fo.Id,
                EmployeeId = fo.EmployeeId,
                Amount = fo.Amount,
                Description = fo.Description,
                CreatedById = fo.CreatedById,
                CreatedAt = fo.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<FinancialOperationDto>(items, totalCount, request.Page, request.PageSize);
    }
}