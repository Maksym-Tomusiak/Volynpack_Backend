using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Services;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file, CancellationToken cancellationToken);
    Task DeleteFileAsync(string fileName, CancellationToken cancellationToken);
}