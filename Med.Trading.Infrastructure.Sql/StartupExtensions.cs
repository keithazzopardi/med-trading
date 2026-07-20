using Med.Shared.Services.SecretProvider;
using Med.Trading.Core.Domain;
using Med.Trading.Infrastructure.Sql.Implementatiosn;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Med.Trading.Infrastructure.Sql;

public static class StartupExtensions
{
    public static void AddSqlImplementations(this IServiceCollection services)
    {
        services.AddDbContext<TradingDbContext>((serviceProvider, options) =>
        {
            var secretProvider = serviceProvider.GetRequiredService<ISecretProvider>();
            options.UseSqlServer(
                secretProvider.GetSecret("ConnectionStrings:SqlConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure());
        });

        services.AddScoped<IInstrumentRepository, InstrumentRepository>();
        services.AddScoped<ITradingOrderRepository, TradingOrderRepository>();

    }
}
