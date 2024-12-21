using LargeFilesManager.Files.Interfaces;

namespace LargeFilesManager.Files.Services
{
    public class FileService : IFileService
    {
        public string CreateEmptyFile(string fileName, string subfolder)
        {
            var fullPath = GenerateFullPathWithFile(fileName, subfolder);

            EnsureDirectoryExists(subfolder);

            File.Create(fullPath).Dispose();

            return fullPath;
        }

        public void AppendToFile(string fullPath, string content)
        {
            EnsureDirectoryExists(Path.GetDirectoryName(fullPath));

            File.AppendAllText(fullPath, content);
        }

        public long GetFileSizeInBytes(string fullPath)
        {
            var fileInfo = new FileInfo(fullPath);
            return fileInfo.Exists ? fileInfo.Length : 0;
        }

        public double GetFileSizeInMb(string fullPath)
        {
            const double bytesPerMb = 1024 * 1024;
            return GetFileSizeInBytes(fullPath) / bytesPerMb;
        }

        public string GenerateFullPathWithFile(string fileName, string subfolder)
        {
            var baseDirectory = GenerateFullPath();

            return Path.Combine($"{baseDirectory}{subfolder}", fileName);
        }

        public async Task<Stream> DownloadAsync(string fileName, string subfolder)
        {
            var fullPath = GenerateFullPathWithFile(fileName, subfolder);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"File not found: {fileName}");
            }

            var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

            await Task.CompletedTask;

            return fileStream;
        }


        public void DeleteFiles(string subfolder)
        {
            var directoryPath = Path.Combine(AppContext.BaseDirectory, subfolder);

            if (Directory.Exists(directoryPath))
            {
                var files = Directory.GetFiles(directoryPath);

                foreach (var file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception if needed
                    }
                }
            }
        }

        public bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        private void EnsureDirectoryExists(string? directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                return;
            }

            if (!Path.IsPathRooted(directoryPath))
            {
                directoryPath = Path.Combine(AppContext.BaseDirectory, directoryPath);
            }

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public List<string> GetFiles(string subfolder)
        {
            var directoryPath = Path.Combine(AppContext.BaseDirectory, subfolder);

            if (Directory.Exists(directoryPath))
            {
                var files = Directory.GetFiles(directoryPath);

                return files.ToList();
            }

            return new List<string>();
        }

        private static string GenerateFullPath()
        {
            var baseDirectory = AppContext.BaseDirectory;

            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }

            return baseDirectory;
        }
    }
}
