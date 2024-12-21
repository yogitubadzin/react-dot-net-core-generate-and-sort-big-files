using System.Collections.Concurrent;
using LargeFilesManager.Core.Interfaces;

namespace LargeFilesManager.Core.Services
{
    public class TimeProgressStorage : ITimeProgressStorage
    {
        private readonly ConcurrentDictionary<string, int> _timeInSeconds = new();

        public Task SetAsync(string key, int seconds)
        {
            return Task.Run(() =>
            {
                _timeInSeconds[key] = seconds;
            });
        }

        public Task<int> GetAsync(string key)
        {
            return Task.FromResult(_timeInSeconds.GetValueOrDefault(key, -1));
        }
    }
}
