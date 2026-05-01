namespace FeatureHub.Api.Models.Auth;

public class RefreshTokenRequest
{
    public required string ExpiredAccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
