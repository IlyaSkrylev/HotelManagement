using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.FinancialOperations;

public record GetEmployeeFinancialStatsQuery(
    long EmployeeId,
    string? StartDate = null,
    string? EndDate = null) : IRequest<EmployeeFinancialStatsDto>;

public class GetEmployeeFinancialStatsQueryHandler : IRequestHandler<GetEmployeeFinancialStatsQuery, EmployeeFinancialStatsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetEmployeeFinancialStatsQueryHandler> _logger;

    public GetEmployeeFinancialStatsQueryHandler(IApplicationDbContext context, ILogger<GetEmployeeFinancialStatsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EmployeeFinancialStatsDto> Handle(GetEmployeeFinancialStatsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос статистики финансов для сотрудника ID: {EmployeeId}, StartDate: {StartDate}, EndDate: {EndDate}",
            request.EmployeeId, request.StartDate, request.EndDate);

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

        var operations = await query.ToListAsync(cancellationToken);

        var totalSalary = operations
            .Where(o => o.Amount > 0)
            .Sum(o => o.Amount);

        var totalFine = operations
            .Where(o => o.Amount < 0)
            .Sum(o => Math.Abs(o.Amount));

        var totalEarned = totalSalary - totalFine;

        return new EmployeeFinancialStatsDto
        {
            TotalSalary = totalSalary,
            TotalFine = totalFine,
            TotalEarned = totalEarned
        };
    }
}