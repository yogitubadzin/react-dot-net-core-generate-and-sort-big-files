namespace LargeFilesManager.Files.Interfaces
{
    public interface IFileProgressStorage
    {
        Task SetAsync(string key, int progress);

        Task<int> GetAsync(string key);
    }
}
