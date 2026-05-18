using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Domain.Entities;
using Paramore.Brighter;

namespace FeatureHub.Application.Environments.Commands.CreateEnvironment;

public class CreateEnvironmentCommand : Command
{
    public string UserId { get; }
    public int ProjectId { get; }
    public string Name { get; }
    public string Token { get; }
    public int AddedEnvironmentId { get; set; }

    public CreateEnvironmentCommand(string userId, int projectId, string name, string token) : base(Guid.NewGuid())
    {
        UserId = userId;
        ProjectId = projectId;
        Name = name;
        Token = token;
    }
}

public class CreateEnvironmentCommandHandler : RequestHandlerAsync<CreateEnvironmentCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateEnvironmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    [ValidateRequest(step: 1)]
    public override async Task<CreateEnvironmentCommand> HandleAsync(CreateEnvironmentCommand command, CancellationToken cancellationToken = default)
    {
        var environment = new Domain.Entities.Environment
        {
            ProjectId = command.ProjectId,
            Name = command.Name,
            Token = command.Token
        };

        _context.Environments.Add(environment);
        await _context.SaveChangesAsync(cancellationToken);

        command.AddedEnvironmentId = environment.Id;

        return await base.HandleAsync(command, cancellationToken);
    }
}
