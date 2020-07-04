using Microsoft.Extensions.DependencyInjection;

namespace web.Auth
{
    public static class AuthorizationExtensions
    {
        public static void AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
                options.AddPolicy(Policies.StberryDev, p => p.RequireRole(Roles.StberryDev)));
        }

        //if you want to add another policy to {options.AddPolicy} from above
        private static void AddPolicy(this IServiceCollection services, string policy, string role)
        {
            services.AddAuthorization(options =>
                options.AddPolicy(policy, p => p.RequireRole(role, Roles.StberryDev)));
        }
    }
}
