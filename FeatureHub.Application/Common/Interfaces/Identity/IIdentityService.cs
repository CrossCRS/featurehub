namespace FeatureHub.Application.Common.Interfaces.Identity;

public interface IIdentityService
{
    Task<string?> LoginAsync(string username, string password);
}
