using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Domain.Entities;
using Paramore.Brighter;

namespace FeatureHub.Application.Projects.Commands.CreateProject;

public class CreateProjectCommand : Command
{
    public string OwnerId { get; }
    public string Name { get; }
    public int AddedProjectId { get; set; }

    public CreateProjectCommand(string ownerId, string name) : base(Guid.NewGuid())
    {
        OwnerId = ownerId;
        Name = name;
    }
}

public class CreateProjectCommandHandler : RequestHandlerAsync<CreateProjectCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    [ValidateRequest(step: 1)]
    public override async Task<CreateProjectCommand> HandleAsync(CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        // TODO: Add validation to check if the user is allowed to create a project (for example for demo user)
        var project = new Project
        {
            OwnerId = command.OwnerId,
            Name = command.Name
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);

        command.AddedProjectId = project.Id;

        return await base.HandleAsync(command, cancellationToken);
    }
}
