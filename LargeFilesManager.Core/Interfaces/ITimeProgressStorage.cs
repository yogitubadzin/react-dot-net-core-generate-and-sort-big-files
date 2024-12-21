namespace LargeFilesManager.Core.Interfaces
{
    public interface ITimeProgressStorage
    {
        Task SetAsync(string key, int seconds);

        Task<int> GetAsync(string key);
    }
}
