using LargeFilesManager.Files.Interfaces;
using System.Collections.Concurrent;

namespace LargeFilesManager.Files.Services
{
    public class FileProgressStorage : IFileProgressStorage
    {
        private readonly ConcurrentDictionary<string, int> _progress = new();

        public Task SetAsync(string key, int progress)
        {
            return Task.Run(() =>
            {
                var clampedProgress = Math.Max(0, Math.Min(progress, 100));
                _progress[key] = clampedProgress;
            });
        }

        public Task<int> GetAsync(string key)
        {
            return Task.FromResult(_progress.GetValueOrDefault(key, -1));
        }
    }
}
