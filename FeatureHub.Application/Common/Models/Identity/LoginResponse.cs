namespace FeatureHub.Application.Common.Models.Identity;

public class LoginResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}