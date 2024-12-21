using LargeFilesManager.Files.Interfaces;
using LargeFilesManager.Files.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LargeFilesManager.Files.Extensions
{
    public static class FilesRegistrationExtensions
    {
        public static void AddFilesServices(this IServiceCollection services)
        {
            services.AddSingleton<IFileGenerator, FileGenerator>();
            services.AddSingleton<IFileProgressStorage, FileProgressStorage>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IFileSorter, FileSorter>();
        }
    }
}
