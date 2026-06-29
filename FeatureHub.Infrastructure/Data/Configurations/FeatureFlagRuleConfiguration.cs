using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeatureHub.Infrastructure.Data.Configurations;

public class FeatureFlagRuleConfiguration : IEntityTypeConfiguration<Domain.Entities.FeatureFlagRule>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.FeatureFlagRule> builder)
    {
        // Exclude soft-deleted feature flag rules
        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.HasOne(r => r.FeatureFlag)
            .WithMany(f => f.Rules)
            .HasForeignKey(r => r.FeatureFlagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(r => r.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.Config)
            .HasMaxLength(4000);

        builder.Property(r => r.IsActive)
            .HasDefaultValue(true);

        builder.Property(r => r.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(r => r.Priority)
            .HasDefaultValue(1);
    }
}