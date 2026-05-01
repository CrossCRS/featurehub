using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces.Identity;
using Paramore.Brighter;

namespace FeatureHub.Application.Auth.Commands.Login;

public class LoginCommand : Command
{
    public string Username { get; }
    public string Password { get; }

    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }

    public LoginCommand(string username, string password) : base(Guid.NewGuid())
    {
        Username = username;
        Password = password;
    }
}

public class LoginCommandHandler : RequestHandlerAsync<LoginCommand>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [ValidateRequest(step: 1)]
    public override async Task<LoginCommand> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var loginResponse = await _identityService.LoginAsync(command.Username, command.Password);

        if (loginResponse == null)
            throw new InvalidCredentialsException();

        command.AccessToken = loginResponse.AccessToken;
        command.RefreshToken = loginResponse.RefreshToken;

        return await base.HandleAsync(command, cancellationToken);
    }
}
