using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeatureHub.Infrastructure.Data.Configurations;

public class FeatureFlagConfiguration : IEntityTypeConfiguration<Domain.Entities.FeatureFlag>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.FeatureFlag> builder)
    {
        // Exclude soft-deleted feature flags
        builder.HasQueryFilter(f => !f.IsDeleted);

        builder.HasOne(f => f.Environment)
            .WithMany(e => e.FeatureFlags)
            .HasForeignKey(f => f.EnvironmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Description)
            .HasMaxLength(500);

        builder.Property(f => f.Value)
            .IsRequired();

        builder.Property(f => f.Data)
            .HasMaxLength(1000);

        builder.Property(f => f.IsActive)
            .HasDefaultValue(true);

        builder.Property(f => f.IsDeleted)
            .HasDefaultValue(false);
    }
}
