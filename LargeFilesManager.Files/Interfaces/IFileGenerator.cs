namespace LargeFilesManager.Files.Interfaces
{
    public interface IFileGenerator
    {
        Task GenerateFileAsync(string fileName, int fileSize, List<string> inputs);
    }
}
