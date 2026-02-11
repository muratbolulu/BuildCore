using BuildCore.WorkflowEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildCore.WorkflowEngine.Infrastructure.Persistence.Configurations;

/// <summary>
/// Transition entity configuration
/// </summary>
public class TransitionConfiguration : IEntityTypeConfiguration<Transition>
{
    public void Configure(EntityTypeBuilder<Transition> builder)
    {
        builder.ToTable("Transitions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Condition)
            .HasMaxLength(2000);

        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
