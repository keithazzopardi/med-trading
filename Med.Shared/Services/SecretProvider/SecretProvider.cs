using Microsoft.Extensions.Configuration;

namespace Med.Shared.Services.SecretProvider;

public sealed class SecretProvider : ISecretProvider
{
    private readonly IConfiguration _configuration;

    public SecretProvider()
    {
        _configuration = CreateConfiguration();
    }

    public string GetSecret(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var environmentSecret = _configuration[name]
                                 ?? Environment.GetEnvironmentVariable(name)
                                 ?? Environment.GetEnvironmentVariable(name.Replace(":", "__", StringComparison.Ordinal));

        if (!string.IsNullOrWhiteSpace(environmentSecret))
        {
            return environmentSecret;
        }

        throw new InvalidOperationException($"Secret '{name}' was not found in configuration or the environment.");
    }

    private static IConfiguration CreateConfiguration()
    {
        var appPath = AppContext.BaseDirectory;
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                              ?? "Development";
        var secretsFileName = $"secrets.{environmentName}.json";

        return new ConfigurationBuilder()
            .SetBasePath(appPath)
            .AddJsonFile(
                Path.Combine(appPath, "Config", secretsFileName),
                optional: true,
                reloadOnChange: true)
            .AddJsonFile(
                Path.Combine(appPath, secretsFileName),
                optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }
}
