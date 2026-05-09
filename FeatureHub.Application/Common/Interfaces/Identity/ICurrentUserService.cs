namespace FeatureHub.Application.Common.Interfaces.Identity;

public interface ICurrentUserService
{
    string GetUserId();
    string GetUserName();
}
