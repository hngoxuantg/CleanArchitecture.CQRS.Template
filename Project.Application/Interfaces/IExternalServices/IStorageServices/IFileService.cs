using Microsoft.AspNetCore.Http;

namespace Project.Application.Interfaces.IExternalServices
{
    public interface IFileService
    {
        Task<string> SaveImageAsync(
            IFormFile formFile,
            string folder,
            CancellationToken cancellationToken = default);

        Task<List<string>> SaveImagesAsync(
            IList<IFormFile> formFiles,
            string folder,
            CancellationToken cancellationToken = default);

        bool DeleteFile(string filePath);

        bool DeleteFiles(IList<string> filePaths);

        bool FileExists(string filePath);

        string GetFileUrl(string relativePath);

        bool IsValidImage(IFormFile formFile);

        string GetAbsoluteUrl(string relativePath);

        IEnumerable<string> GetAbsoluteUrls(IEnumerable<string> relativePaths);
    }
}