using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace web.Configuration
{
    public static class Options
    {
        public static void AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions()
                .Configure<JwtOptions>(o => configuration.GetSection("Jwt").Bind(o));
        }
    }
}
