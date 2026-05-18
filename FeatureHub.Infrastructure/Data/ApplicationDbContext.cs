using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Domain.Common;
using FeatureHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FeatureHub.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public DbSet<Domain.Entities.Project> Projects => Set<Domain.Entities.Project>();
    public DbSet<Domain.Entities.Environment> Environments => Set<Domain.Entities.Environment>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<BaseAuditableEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = entry.Entity.UpdatedAt;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
