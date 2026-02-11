using BuildCore.WorkflowEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildCore.WorkflowEngine.Infrastructure.Persistence.Configurations;

/// <summary>
/// WorkflowInstanceHistory entity configuration
/// </summary>
public class WorkflowInstanceHistoryConfiguration : IEntityTypeConfiguration<WorkflowInstanceHistory>
{
    public void Configure(EntityTypeBuilder<WorkflowInstanceHistory> builder)
    {
        builder.ToTable("WorkflowInstanceHistories");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(h => h.UserId)
            .HasMaxLength(100);

        builder.Property(h => h.Notes)
            .HasMaxLength(1000);

        builder.Property(h => h.ActionDate)
            .IsRequired();

        builder.HasQueryFilter(h => !h.IsDeleted);
    }
}
