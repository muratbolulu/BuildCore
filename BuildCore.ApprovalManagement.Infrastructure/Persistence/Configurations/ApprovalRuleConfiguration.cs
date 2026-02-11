using BuildCore.ApprovalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildCore.ApprovalManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// ApprovalRule entity configuration
/// </summary>
public class ApprovalRuleConfiguration : IEntityTypeConfiguration<ApprovalRule>
{
    public void Configure(EntityTypeBuilder<ApprovalRule> builder)
    {
        builder.ToTable("ApprovalRules");

        builder.HasKey(ar => ar.Id);

        builder.Property(ar => ar.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ar => ar.Description)
            .HasMaxLength(1000);

        builder.Property(ar => ar.RuleType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ar => ar.Condition)
            .HasMaxLength(2000);

        builder.Property(ar => ar.IsActive)
            .IsRequired();

        // Relationships
        builder.HasMany(ar => ar.ApprovalChains)
            .WithOne(ac => ac.ApprovalRule)
            .HasForeignKey(ac => ac.ApprovalRuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ar => ar.ApprovalRequests)
            .WithOne(req => req.ApprovalRule)
            .HasForeignKey(req => req.ApprovalRuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(ar => !ar.IsDeleted);
    }
}
