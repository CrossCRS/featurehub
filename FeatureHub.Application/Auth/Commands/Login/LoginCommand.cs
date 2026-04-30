using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Interfaces.Identity;
using Paramore.Brighter;

namespace FeatureHub.Application.Auth.Commands.Login;

public class LoginCommand : Command
{
    public string Username { get; }
    public string Password { get; }

    public string? Token { get; set; }

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
        command.Token = await _identityService.LoginAsync(command.Username, command.Password);

        if (command.Token == null)
            throw new UnauthorizedAccessException("Invalid username or password.");

        return await base.HandleAsync(command, cancellationToken);
    }
}
