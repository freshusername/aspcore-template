using Microsoft.Extensions.DependencyInjection;
using service.Interfaces;
using service.Services;

namespace service
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IRecordsService, RecordsService>();
            return services;
        }
    }
}
