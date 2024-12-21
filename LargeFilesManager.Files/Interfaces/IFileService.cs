namespace LargeFilesManager.Files.Interfaces
{
    public interface IFileService
    {
        string CreateEmptyFile(string fileName, string subfolder);

        void AppendToFile(string fullPath, string content);

        long GetFileSizeInBytes(string fullPath);

        double GetFileSizeInMb(string fullPath);

        string GenerateFullPathWithFile(string fileName, string subfolder);

        Task<Stream> DownloadAsync(string fileName, string subfolder);

        bool FileExists(string fileName);

        void DeleteFiles(string subfolder);

        List<string> GetFiles(string subfolder);
    }
}
