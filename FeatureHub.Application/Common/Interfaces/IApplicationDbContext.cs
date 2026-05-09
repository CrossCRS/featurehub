using FeatureHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FeatureHub.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Project> Projects { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
