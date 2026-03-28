using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Services;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file, string subFolder, CancellationToken cancellationToken);
    Task DeleteFileAsync(string fileUrl, string subFolder, CancellationToken cancellationToken);
}