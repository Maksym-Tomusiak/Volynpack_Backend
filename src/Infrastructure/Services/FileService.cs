using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces.Services;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class FileService : IFileService
{
    private readonly string _uploadPath;
    
    public FileService(IOptions<FileStorageSettings> settings)
    {
        _uploadPath = settings.Value.UploadPath;
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<string> SaveFileAsync(IFormFile file, string subFolder, CancellationToken cancellationToken)
    {
        var finalPath = Path.Combine(_uploadPath, subFolder);
        if (!Directory.Exists(finalPath))
        {
            Directory.CreateDirectory(finalPath);
        }

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(finalPath, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        return fileName;
    }

    public async Task DeleteFileAsync(string fileUrl, string subFolder, CancellationToken cancellationToken)
    {
        var fileName = Path.GetFileName(fileUrl); 
        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }
        var filePath = Path.Combine(_uploadPath, subFolder, fileName);
        if (File.Exists(filePath))
        {
            await Task.Run(() => File.Delete(filePath), cancellationToken);
        }
    }
}