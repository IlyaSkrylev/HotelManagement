using HotelManagement.Domain.Entities.Abstractions;

namespace HotelManagement.Domain.Entities;

public class User : IEntity
{
    public long Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Patronymic { get; set; }
    public string? Phone { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? LastLogin { get; set; }

    // Refresh Token поля
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Навигационные свойства
    public ICollection<UserHotelRole> UserHotelRoles { get; set; } = new List<UserHotelRole>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
}