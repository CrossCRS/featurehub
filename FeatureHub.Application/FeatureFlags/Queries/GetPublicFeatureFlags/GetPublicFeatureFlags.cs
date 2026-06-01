using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.DTOs.FeatureFlag;
using FeatureHub.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;

namespace FeatureHub.Application.FeatureFlags.Queries.GetPublicFeatureFlags;

public class GetPublicFeatureFlags : IQuery<IEnumerable<PublicFeatureFlagDto>>
{
    public string EnvironmentToken { get; }
    public string? ClientHash { get; }

    public GetPublicFeatureFlags(string environmentToken, string? clientHash)
    {
        EnvironmentToken = environmentToken;
        ClientHash = clientHash;
    }
}

public class GetPublicFeatureFlagsHandler : QueryHandlerAsync<GetPublicFeatureFlags, IEnumerable<PublicFeatureFlagDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPublicFeatureFlagsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    [ValidateRequest(step: 1)]
    public override async Task<IEnumerable<PublicFeatureFlagDto>> ExecuteAsync(GetPublicFeatureFlags query, CancellationToken cancellationToken)
    {
        var environment = _context.Environments
            .Where(e => e.Token == query.EnvironmentToken)
            .Include(e => e.FeatureFlags)
            .AsNoTracking()
            .FirstOrDefault();

        if (environment == null)
        {
            throw new ArgumentException("Invalid environment token.");
        }

        var featureFlags = environment.FeatureFlags.Select(ff => new PublicFeatureFlagDto
        {
            Name = ff.Name,
            Value = ff.Value, // TODO: Implement targeting rules and percentage rollouts based on the ClientHash
            Data = ff.Data
        }).ToList();

        return featureFlags;
    }
}