namespace HotelManagement.Application.Settings;

public class ServerSettings
{
    public const string SectionName = "ServerSettings";
    public string Host { get; set; } = "0.0.0.0";
    public int Port { get; set; } = 5030;
    public string PublicHost { get; set; } = "localhost";
    public int PublicPort { get; set; } = 5030;
    public List<string> AllowedOrigins { get; set; } = new();

    public string PublicUrl => $"http://{PublicHost}:{PublicPort}";
}