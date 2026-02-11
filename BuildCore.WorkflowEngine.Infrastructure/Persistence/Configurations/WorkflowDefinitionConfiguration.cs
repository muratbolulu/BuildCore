using BuildCore.WorkflowEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildCore.WorkflowEngine.Infrastructure.Persistence.Configurations;

/// <summary>
/// WorkflowDefinition entity configuration
/// </summary>
public class WorkflowDefinitionConfiguration : IEntityTypeConfiguration<WorkflowDefinition>
{
    public void Configure(EntityTypeBuilder<WorkflowDefinition> builder)
    {
        builder.ToTable("WorkflowDefinitions");

        builder.HasKey(wd => wd.Id);

        builder.Property(wd => wd.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(wd => wd.Description)
            .HasMaxLength(1000);

        builder.Property(wd => wd.Version)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(wd => wd.IsActive)
            .IsRequired();

        // Relationships
        builder.HasMany(wd => wd.Steps)
            .WithOne(s => s.WorkflowDefinition)
            .HasForeignKey(s => s.WorkflowDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(wd => wd.Instances)
            .WithOne(wi => wi.WorkflowDefinition)
            .HasForeignKey(wi => wi.WorkflowDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(wd => !wd.IsDeleted);
    }
}
