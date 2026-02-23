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

    public async Task<string> SaveFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(_uploadPath, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        return fileName;
    }

    public async Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken)
    {
        var fileName = Path.GetFileName(fileUrl); 
        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }
        var filePath = Path.Combine(_uploadPath, fileName);
        if (File.Exists(filePath))
        {
            await Task.Run(() => File.Delete(filePath), cancellationToken);
        }
    }
}