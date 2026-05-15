namespace HotelManagement.Application.DTOs;

public class FinancialOperationDto
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public int Amount { get; set; }
    public string? Description { get; set; }
    public long CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class FinancialOperationWithEmployeeDto
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string? Description { get; set; }
    public long CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class EmployeeFinancialStatsDto
{
    public int TotalSalary { get; set; } 
    public int TotalFine { get; set; }     
    public int TotalEarned { get; set; }   
}