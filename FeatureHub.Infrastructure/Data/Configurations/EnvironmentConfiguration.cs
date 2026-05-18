using FeatureHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeatureHub.Infrastructure.Data.Configurations;

public class EnvironmentConfiguration : IEntityTypeConfiguration<Domain.Entities.Environment>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Environment> builder)
    {
        // Exclude soft-deleted environments
        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(65);

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);
    }
}
