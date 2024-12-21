using LargeFilesManager.BL.Interfaces.Files;
using LargeFilesManager.BL.Models;
using LargeFilesManager.Core.Interfaces;
using LargeFilesManager.Files.Constants;
using LargeFilesManager.Files.Interfaces;
using LargeFilesManager.StringsGeneration.Interfaces;

namespace LargeFilesManager.BL.Services.Files
{
    public class FileGenerationService : IFileGenerationService
    {
        private readonly Dictionary<int, string> _fileSizeMappings = new()
        {
            { 1, "1MB" },
            { 5, "5MB" },
            { 10, "10MB" },
            { 50, "50MB" },
            { 100, "100MB" },
            { 500, "500MB" },
            { 1000, "1G" },
            { 5000, "5G" },
            { 10000, "10G" },
            { 50000, "50G" },
            { 100000, "100G" },
        };

        private readonly IFileGenerator _fileGenerator;
        private readonly IDateTimeService _dateTimeService;
        private readonly ISentencesGenerator _sentencesGenerator;
        private readonly IFileProgressStorage _fileProgressStorage;
        private readonly IFileService _fileService;

        public FileGenerationService(
            IFileGenerator fileGenerator,
            IDateTimeService dateTimeService,
            ISentencesGenerator sentencesGenerator,
            IFileProgressStorage fileProgressStorage,
            IFileService fileService)
        {
            _fileGenerator = fileGenerator;
            _dateTimeService = dateTimeService;
            _sentencesGenerator = sentencesGenerator;
            _fileProgressStorage = fileProgressStorage;
            _fileService = fileService;
        }

        public async Task<FileStatusResponse> GenerateAsync(int fileSize)
        {
            var fileName = GenerateFileName(fileSize);

            await _fileProgressStorage.SetAsync(fileName, 0);

            DeleteFiles();

            var sentences = _sentencesGenerator.Generate(100);

            _ = Task.Run(() => _fileGenerator.GenerateFileAsync(fileName, fileSize, sentences));

            var status = await _fileProgressStorage.GetAsync(fileName);

            return new FileStatusResponse
            {
                FileName = fileName,
                Status = status
            };
        }

        public async Task<FileStatusResponse> GetFileStatusAsync(string fileName)
        {
            var progress = await _fileProgressStorage.GetAsync(fileName);

            return new FileStatusResponse
            {
                FileName = fileName,
                Status = progress
            };
        }

        public async Task<Stream> DownloadAsync(string fileName)
        {
            return await _fileService.DownloadAsync(fileName, FileConstants.FileGeneratorPath);
        }

        public void DeleteFiles()
        {
            _fileService.DeleteFiles(FileConstants.FileGeneratorPath);
        }

        private string GenerateFileName(int fileSize)
        {
            var now = _dateTimeService.UtcNow;
            var datePart = now.ToString("MM_dd_yyyy");

            var fileSizeReadable = _fileSizeMappings.ContainsKey(fileSize)
                ? _fileSizeMappings[fileSize]
                : $"{fileSize}Unknown";

            var fileName = $"GeneratedFile_{fileSizeReadable}_{datePart}.txt";

            return fileName;
        }
    }
}
