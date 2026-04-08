using HotelManagement.Application.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IWebHostEnvironment webHostEnvironment, ILogger<FileStorageService> logger)
    {
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string subFolder, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("File is null or empty");
            return string.Empty;
        }

        _logger.LogInformation("Saving file: {FileName}, size: {Size} bytes", file.FileName, file.Length);

        var basePath = _webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath;
        var uploadsFolder = Path.Combine(basePath, "uploads", subFolder);

        _logger.LogInformation("Upload folder: {Folder}", uploadsFolder);

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
            _logger.LogInformation("Created folder: {Folder}", uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        _logger.LogInformation("Saving to: {FilePath}", filePath);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream, cancellationToken);
        }

        var relativePath = $"/uploads/{subFolder}/{uniqueFileName}";
        _logger.LogInformation("Relative path: {RelativePath}", relativePath);

        return relativePath;
    }

    public void DeleteFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            _logger.LogWarning("DeleteFile: filePath is null or empty");
            return;
        }

        var basePath = _webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath;
        var fullPath = Path.Combine(basePath, filePath.TrimStart('/'));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("Deleted file: {FullPath}", fullPath);
        }
        else
        {
            _logger.LogWarning("File not found: {FullPath}", fullPath);
        }
    }
}