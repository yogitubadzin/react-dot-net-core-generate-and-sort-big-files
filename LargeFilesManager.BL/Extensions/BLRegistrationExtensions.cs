using LargeFilesManager.BL.Interfaces.Files;
using LargeFilesManager.BL.Services.Files;
using Microsoft.Extensions.DependencyInjection;

namespace LargeFilesManager.BL.Extensions
{
    public static class BLRegistrationExtensions
    {
        public static void AddBLServices(this IServiceCollection services)
        {
            services.AddSingleton<IFileGenerationService, FileGenerationService>();
            services.AddSingleton<IFileSortingService, FileSortingService>();
        }
    }
}
