using LargeFilesManager.StringsGeneration.Interfaces;
using LargeFilesManager.StringsGeneration.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LargeFilesManager.StringsGeneration.Extensions
{
    public static class StringsGenerationRegistrationExtensions
    {
        public static void AddStringsGenerationServices(this IServiceCollection services)
        {
            services.AddSingleton<ISentencesGenerator, SentencesGenerator>();
        }
    }
}
