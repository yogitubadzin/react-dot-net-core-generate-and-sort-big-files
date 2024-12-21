using LargeFilesManager.Core.Interfaces;
using LargeFilesManager.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LargeFilesManager.Core.Extensions
{
    public static class CoreRegistrationExtensions
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddSingleton<IDateTimeService, DateTimeService>();
            services.AddSingleton<ITimeProgressStorage, TimeProgressStorage>();
        }
    }
}
