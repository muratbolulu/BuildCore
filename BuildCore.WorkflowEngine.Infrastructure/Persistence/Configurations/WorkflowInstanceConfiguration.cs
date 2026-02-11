using BuildCore.WorkflowEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildCore.WorkflowEngine.Infrastructure.Persistence.Configurations;

/// <summary>
/// WorkflowInstance entity configuration
/// </summary>
public class WorkflowInstanceConfiguration : IEntityTypeConfiguration<WorkflowInstance>
{
    public void Configure(EntityTypeBuilder<WorkflowInstance> builder)
    {
        builder.ToTable("WorkflowInstances");

        builder.HasKey(wi => wi.Id);

        builder.Property(wi => wi.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(wi => wi.InitiatorUserId)
            .HasMaxLength(100);

        builder.Property(wi => wi.ContextData)
            .HasMaxLength(4000);

        // Relationships
        builder.HasOne(wi => wi.CurrentStep)
            .WithMany()
            .HasForeignKey(wi => wi.CurrentStepId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(wi => wi.History)
            .WithOne(h => h.WorkflowInstance)
            .HasForeignKey(h => h.WorkflowInstanceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(wi => !wi.IsDeleted);
    }
}
