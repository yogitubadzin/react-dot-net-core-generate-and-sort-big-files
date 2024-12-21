using System.Diagnostics;
using LargeFilesManager.Files.Constants;
using LargeFilesManager.Files.Interfaces;
using System.Text.RegularExpressions;
using LargeFilesManager.Core.Interfaces;

public class FileSorter : IFileSorter
{
    private readonly IFileService _fileService;
    private readonly IFileProgressStorage _progressStorage;
    private readonly ITimeProgressStorage _timeProgressStorage;

    public FileSorter(IFileService fileService, IFileProgressStorage progressStorage, ITimeProgressStorage timeProgressStorage)
    {
        _fileService = fileService;
        _progressStorage = progressStorage;
        _timeProgressStorage = timeProgressStorage;
    }

    public async Task SortFileAsync(string oldFileName, string newFileName)
    {
        _fileService.CreateEmptyFile(newFileName, FileConstants.FileSortPath);

        _fileService.DeleteFiles(FileConstants.FileSortTempPath);

        var totalSize = _fileService.GetFileSizeInBytes(_fileService.GenerateFullPathWithFile(oldFileName, FileConstants.FileGeneratorPath));
        long processedBytes = 0;

        // Total time of processing is static divided on 3 different sections: File split, Sorting files, Merging files

        // File Splitting and Appending (60%)
        var keyFiles = await SplitFileByKeyAsync(oldFileName, totalSize, progress =>
        {
            _progressStorage.SetAsync(newFileName, progress);
        });

        // Sorting Files (30%)
        await SortKeyFilesAsync(keyFiles, newFileName);

        // Merging Files (10%)
        await MergeSortedKeyFilesAsync(keyFiles, newFileName);

        _fileService.DeleteFiles(FileConstants.FileSortTempPath);
    }

    private async Task SortKeyFilesAsync(List<string> keyFiles, string newFileName)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        const int maxConcurrentTasks = 5;
        using var semaphore = new SemaphoreSlim(maxConcurrentTasks);
        var tasks = new List<Task>();
        int sortedFiles = 0;

        foreach (var keyFile in keyFiles)
        {
            await semaphore.WaitAsync();
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    await SortKeyFileAsync(keyFile);

                    Interlocked.Increment(ref sortedFiles);
                    int progress = 60 + (sortedFiles * 30) / keyFiles.Count;
                    await _progressStorage.SetAsync(newFileName, progress);
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks);

        stopWatch.Stop();
        await _timeProgressStorage.SetAsync(newFileName, stopWatch.Elapsed.Seconds);
    }

    private async Task<List<string>> SplitFileByKeyAsync(string oldFileName, long totalSize, Action<int> reportProgress)
    {
        var keyFiles = new Dictionary<string, string>();
        var bufferMap = new Dictionary<string, List<string>>();
        const int initialBufferFlushThreshold = 3000;
        int dynamicBufferFlushThreshold = initialBufferFlushThreshold;

        long processedBytes = 0;

        using var stream = await _fileService.DownloadAsync(oldFileName, FileConstants.FileGeneratorPath);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line == null) continue;

            processedBytes += line.Length + Environment.NewLine.Length;

            int progress = (int)((processedBytes * 60) / totalSize);
            reportProgress(progress);

            var key = ExtractKey(line);

            if (!bufferMap.ContainsKey(key))
            {
                bufferMap[key] = new List<string>();

                if (!keyFiles.ContainsKey(key))
                {
                    var tempFileName = $"{key.Replace(' ', '_')}.txt";
                    var tempFilePath = _fileService.GenerateFullPathWithFile(tempFileName, FileConstants.FileSortTempPath);
                    keyFiles[key] = tempFilePath;
                    _fileService.CreateEmptyFile(tempFileName, FileConstants.FileSortTempPath);
                }
            }

            bufferMap[key].Add(line);

            if (bufferMap[key].Count >= dynamicBufferFlushThreshold)
            {
                await FlushBufferToFileAsync(keyFiles[key], bufferMap[key]);
                bufferMap[key].Clear();
            }

            dynamicBufferFlushThreshold = Math.Max(initialBufferFlushThreshold / Math.Max(bufferMap.Count, 1), 50);
        }

        foreach (var key in bufferMap.Keys)
        {
            if (bufferMap[key].Count > 0)
            {
                await FlushBufferToFileAsync(keyFiles[key], bufferMap[key]);
                bufferMap[key].Clear();
            }
        }

        return keyFiles.Values.ToList();
    }

    private async Task FlushBufferToFileAsync(string filePath, List<string> buffer)
    {
        var content = string.Join(Environment.NewLine, buffer);
        await File.AppendAllTextAsync(filePath, content + Environment.NewLine);
    }

    private async Task SortKeyFileAsync(string keyFilePath)
    {
        var lines = await File.ReadAllLinesAsync(keyFilePath);

        Array.Sort(lines, (x, y) =>
        {
            var xParts = x.Split('.', 2);
            var yParts = y.Split('.', 2);

            var xString = xParts.Length > 1 ? xParts[1].Trim() : x;
            var yString = yParts.Length > 1 ? yParts[1].Trim() : y;

            var stringComparison = string.Compare(xString, yString, StringComparison.Ordinal);
            if (stringComparison != 0) return stringComparison;

            var xId = ExtractNumber(xParts[0]);
            var yId = ExtractNumber(yParts[0]);

            return xId.CompareTo(yId);
        });

        await File.WriteAllLinesAsync(keyFilePath, lines);
    }

    private async Task MergeSortedKeyFilesAsync(List<string> keyFilePaths, string newFileName)
    {
        using var writer = new StreamWriter(_fileService.GenerateFullPathWithFile(newFileName, FileConstants.FileSortPath));
        var readers = new List<StreamReader>();

        try
        {
            foreach (var keyFilePath in keyFilePaths.OrderBy(x => x))
            {
                readers.Add(new StreamReader(keyFilePath));
            }

            var priorityQueue = new SortedDictionary<string, Queue<(StreamReader reader, string line)>>();
            long totalLines = 0;
            long processedLines = 0;

            foreach (var reader in readers)
            {
                while (!reader.EndOfStream)
                {
                    await reader.ReadLineAsync();
                    totalLines++;
                }
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                reader.DiscardBufferedData();
            }

            foreach (var reader in readers)
            {
                var line = await reader.ReadLineAsync();
                if (line == null) continue;

                var key = ExtractKey(line);
                if (!priorityQueue.ContainsKey(key))
                {
                    priorityQueue[key] = new Queue<(StreamReader reader, string line)>();
                }
                priorityQueue[key].Enqueue((reader, line));
            }

            while (priorityQueue.Count > 0)
            {
                var smallestKey = priorityQueue.First();
                var (reader, line) = smallestKey.Value.Dequeue();

                await writer.WriteLineAsync(line);
                processedLines++;

                var progress = 90 + (int)((processedLines * 10) / totalLines);
                await _progressStorage.SetAsync(newFileName, progress);

                if (smallestKey.Value.Count == 0)
                {
                    priorityQueue.Remove(smallestKey.Key);
                }

                var nextLine = await reader.ReadLineAsync();
                if (nextLine != null)
                {
                    var nextKey = ExtractKey(nextLine);
                    if (!priorityQueue.ContainsKey(nextKey))
                    {
                        priorityQueue[nextKey] = new Queue<(StreamReader reader, string line)>();
                    }
                    priorityQueue[nextKey].Enqueue((reader, nextLine));
                }
            }
        }
        finally
        {
            foreach (var reader in readers)
            {
                reader.Dispose();
            }
        }

        await _progressStorage.SetAsync(newFileName, 100);
    }

    private string ExtractKey(string line)
    {
        var parts = line.Split('.');
        if (parts.Length < 2)
        {
            return line;
        }

        var keySection = parts[1].Trim();
        var words = keySection.Split(' ');
        return words.Length > 0 ? words[0] : keySection;
    }

    private int ExtractNumber(string line)
    {
        var match = Regex.Match(line, "\\d+");
        return match.Success ? int.Parse(match.Value) : int.MaxValue;
    }
}
