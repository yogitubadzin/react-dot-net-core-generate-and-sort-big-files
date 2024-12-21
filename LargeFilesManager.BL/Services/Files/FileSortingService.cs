using LargeFilesManager.BL.Interfaces.Files;
using LargeFilesManager.BL.Models;
using LargeFilesManager.Core.Interfaces;
using LargeFilesManager.Files.Constants;
using LargeFilesManager.Files.Interfaces;

namespace LargeFilesManager.BL.Services.Files
{
    public class FileSortingService : IFileSortingService
    {
        private readonly IFileProgressStorage _fileProgressStorage;
        private readonly IFileService _fileService;
        private readonly IFileSorter _fileSorter;
        private readonly ITimeProgressStorage _timeProgressStorage;

        public FileSortingService(
            IFileProgressStorage fileProgressStorage,
            IFileService fileService,
            IFileSorter fileSorter,
            ITimeProgressStorage timeProgressStorage)
        {
            _fileProgressStorage = fileProgressStorage;
            _fileService = fileService;
            _fileSorter = fileSorter;
            _timeProgressStorage = timeProgressStorage;
        }

        public bool FileExists()
        {
            var files = GetFiles();

            return files.Any();
        }

        public async Task<FileSortResponse> SortAsync()
        {
            var files = GenerateFileName();
            if (files == default)
            {
                return new FileSortResponse();
            }

            await _fileProgressStorage.SetAsync(files.NewFile, 0);

            DeleteFiles();

            _ = Task.Run(() => _fileSorter.SortFileAsync(files.OldFile, files.NewFile));

            var status = await _fileProgressStorage.GetAsync(files.NewFile);
            var sortTimeInSeconds = await _timeProgressStorage.GetAsync(files.NewFile);

            return new FileSortResponse
            {
                FileName = files.NewFile,
                Status = status,
                SortTimeInSeconds = sortTimeInSeconds
            };
        }

        public async Task<FileSortResponse> GetFileSortAsync(string fileName)
        {
            var progress = await _fileProgressStorage.GetAsync(fileName);
            var sortTimeInSeconds = await _timeProgressStorage.GetAsync(fileName);

            return new FileSortResponse
            {
                FileName = fileName,
                Status = progress,
                SortTimeInSeconds = sortTimeInSeconds
            };
        }

        public async Task<Stream> DownloadAsync(string fileName)
        {
            return await _fileService.DownloadAsync(fileName, FileConstants.FileSortPath);
        }

        public void DeleteFiles()
        {
            _fileService.DeleteFiles(FileConstants.FileSortTempPath);
            _fileService.DeleteFiles(FileConstants.FileSortPath);
        }

        private (string OldFile, string NewFile) GenerateFileName()
        {
            var files = GetFiles();

            if (files.Any())
            {
                var file = files.First();
                var fileName = Path.GetFileName(file);
                return (fileName, $"Sorted_{fileName}");
            }

            return (null, null);
        }

        private List<string> GetFiles()
        {
            return _fileService.GetFiles(FileConstants.FileGeneratorPath);
        }
    }
}
