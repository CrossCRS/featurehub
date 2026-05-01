using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces.Identity;
using Paramore.Brighter;

namespace FeatureHub.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : Command
{
    public string ExpiredAccessToken { get; }
    public string RefreshToken { get; }

    public string? NewAccessToken { get; set; }
    public string? NewRefreshToken { get; set; }

    public RefreshTokenCommand(string expiredAccessToken, string refreshToken) : base(Guid.NewGuid())
    {
        ExpiredAccessToken = expiredAccessToken;
        RefreshToken = refreshToken;
    }
}

public class RefreshTokenCommandHandler : RequestHandlerAsync<RefreshTokenCommand>
{
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [ValidateRequest(step: 1)]
    public override async Task<RefreshTokenCommand> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        var refreshTokenResponse = await _identityService.RefreshTokenAsync(command.ExpiredAccessToken, command.RefreshToken);

        if (refreshTokenResponse == null)
            throw new InvalidCredentialsException("Invalid access or refresh token.");

        command.NewAccessToken = refreshTokenResponse.AccessToken;
        command.NewRefreshToken = refreshTokenResponse.RefreshToken;

        return await base.HandleAsync(command, cancellationToken);
    }
}