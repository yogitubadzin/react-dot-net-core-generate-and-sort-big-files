using LargeFilesManager.BL.Models;

namespace LargeFilesManager.BL.Interfaces.Files
{
    public interface IFileSortingService
    {
        bool FileExists();

        Task<FileSortResponse> SortAsync();

        Task<FileSortResponse> GetFileSortAsync(string fileName);

        Task<Stream> DownloadAsync(string fileName);

        void DeleteFiles();
    }
}
