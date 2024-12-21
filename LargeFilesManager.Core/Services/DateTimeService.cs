using LargeFilesManager.Core.Interfaces;

namespace LargeFilesManager.Core.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime UtcNow { get; } = DateTime.UtcNow;
    }
}
