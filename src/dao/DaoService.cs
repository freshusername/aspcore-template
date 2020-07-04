using dao.Interfaces;
using dao.Repos;
using Microsoft.Extensions.DependencyInjection;

namespace dao
{
    public static class DaoService
    {
        public static IServiceCollection AddDaoServices(this IServiceCollection services)
        {
            services.AddTransient<IRecordsRepo, RecordsRepo>();
            return services;
        }
    }
}
