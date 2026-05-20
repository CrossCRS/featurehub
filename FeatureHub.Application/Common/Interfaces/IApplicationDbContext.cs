using Microsoft.EntityFrameworkCore;

namespace FeatureHub.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Domain.Entities.Project> Projects { get; }
    DbSet<Domain.Entities.Environment> Environments { get; }
    DbSet<Domain.Entities.FeatureFlag> FeatureFlags { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
