namespace LargeFilesManager.Files.Services;

using LargeFilesManager.Files.Constants;
using LargeFilesManager.Files.Interfaces;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class FileGenerator : IFileGenerator
{
    private readonly IFileProgressStorage _progressStorage;
    private readonly IFileService _fileService;

    public FileGenerator(IFileProgressStorage progressStorage, IFileService fileService)
    {
        _progressStorage = progressStorage;
        _fileService = fileService;
    }

    public async Task GenerateFileAsync(string fileName, int maxFileSizeInMb, List<string> inputs)
    {
        var fileLock = new SemaphoreSlim(1, 1);
        var semaphore = new SemaphoreSlim(5);
        var random = new Random();
        const int bytesPerLine = 50;
        const int linesPerBatch = 1000;

        var fullPath = _fileService.CreateEmptyFile(fileName, FileConstants.FileGeneratorPath);
        await _progressStorage.SetAsync(fileName, 0);

        var tasks = new List<Task>();
        bool sizeExceeded = false;

        for (int batchIndex = 0; !sizeExceeded; batchIndex++)
        {
            await semaphore.WaitAsync();
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var batch = new StringBuilder();

                    for (var j = 0; j < linesPerBatch; j++)
                    {
                        var number = random.Next(1, 1000);
                        var text = inputs[random.Next(inputs.Count)];
                        batch.AppendLine($"{number}. {text}");
                    }

                    var content = batch.ToString();

                    await fileLock.WaitAsync();
                    try
                    {
                        var currentSizeInMb = _fileService.GetFileSizeInMb(fullPath);

                        if (currentSizeInMb >= maxFileSizeInMb)
                        {
                            sizeExceeded = true;
                            await _progressStorage.SetAsync(fileName, 100);
                            return;
                        }

                        _fileService.AppendToFile(fullPath, content);

                        var currentSizeInMbSecondTime = _fileService.GetFileSizeInMb(fullPath);

                        var progress = (int)((currentSizeInMbSecondTime / maxFileSizeInMb) * 100);
                        await _progressStorage.SetAsync(fileName, progress);
                    }
                    finally
                    {
                        fileLock.Release();
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks);

        await _progressStorage.SetAsync(fileName, 100);
    }
}
