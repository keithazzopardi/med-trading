using Med.Shared.Services;
using Med.Shared.Services.SecretProvider;
using Med.Shared.Services.UserContext;
using Microsoft.Extensions.DependencyInjection;

namespace Med.Shared
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSharedImplementations(this IServiceCollection services)
        {
           
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();
            services.AddSingleton<ISecretProvider, SecretProvider>();

            return services;
        }
    }
}
