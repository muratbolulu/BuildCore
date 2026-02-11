using BuildCore.WorkflowEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildCore.WorkflowEngine.Infrastructure.Persistence.Configurations;

/// <summary>
/// WorkflowStep entity configuration
/// </summary>
public class WorkflowStepConfiguration : IEntityTypeConfiguration<WorkflowStep>
{
    public void Configure(EntityTypeBuilder<WorkflowStep> builder)
    {
        builder.ToTable("WorkflowSteps");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.StepType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.AssigneeRole)
            .HasMaxLength(100);

        builder.Property(s => s.AssigneeUserId)
            .HasMaxLength(100);

        builder.Property(s => s.Order)
            .IsRequired();

        builder.Property(s => s.IsRequired)
            .IsRequired();

        // Relationships
        builder.HasMany(s => s.OutgoingTransitions)
            .WithOne(t => t.FromStep)
            .HasForeignKey(t => t.FromStepId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.IncomingTransitions)
            .WithOne(t => t.ToStep)
            .HasForeignKey(t => t.ToStepId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
