using LargeFilesManager.BL.Models;

namespace LargeFilesManager.BL.Interfaces.Files
{
    public interface IFileGenerationService
    {
        Task<FileStatusResponse> GenerateAsync(int fileSize);

        Task<FileStatusResponse> GetFileStatusAsync(string fileName);

        Task<Stream> DownloadAsync(string fileName);

        void DeleteFiles();
    }
}
