using Microsoft.AspNetCore.Http;

namespace HotelManagement.Application.Abstractions;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string subFolder, CancellationToken cancellationToken);
    void DeleteFile(string filePath);
}