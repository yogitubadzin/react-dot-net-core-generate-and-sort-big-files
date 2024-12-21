namespace LargeFilesManager.Files.Interfaces
{
    public interface IFileSorter
    {
        Task SortFileAsync(string oldFileName, string newFileName);
    }
}
