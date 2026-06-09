using Microsoft.AspNetCore.Http;

namespace Project.Application.Common.Interfaces.IExternalServices.IStorageServices
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(
            IFormFile formFile,
            string folder,
            CancellationToken cancellationToken = default);

        Task<List<string>> SaveFilesAsync(
            IList<IFormFile> formFiles,
            string folder,
            CancellationToken cancellationToken = default);

        bool DeleteFile(string filePath);

        bool DeleteFiles(IList<string> filePaths);

        bool FileExists(string filePath);

        string GetFileUrl(string relativePath);

        bool IsValidFile(IFormFile formFile);

        string GetAbsoluteUrl(string relativePath);

        IEnumerable<string> GetAbsoluteUrls(IEnumerable<string> relativePaths);
    }
}