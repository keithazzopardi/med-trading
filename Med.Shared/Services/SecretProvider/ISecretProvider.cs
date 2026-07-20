namespace Med.Shared.Services.SecretProvider;

public interface ISecretProvider
{
    string GetSecret(string name);
}
