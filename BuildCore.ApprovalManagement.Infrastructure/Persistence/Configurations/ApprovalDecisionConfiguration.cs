using BuildCore.ApprovalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildCore.ApprovalManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// ApprovalDecision entity configuration
/// </summary>
public class ApprovalDecisionConfiguration : IEntityTypeConfiguration<ApprovalDecision>
{
    public void Configure(EntityTypeBuilder<ApprovalDecision> builder)
    {
        builder.ToTable("ApprovalDecisions");

        builder.HasKey(ad => ad.Id);

        builder.Property(ad => ad.ApproverUserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ad => ad.ApproverRole)
            .HasMaxLength(100);

        builder.Property(ad => ad.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ad => ad.Notes)
            .HasMaxLength(1000);

        builder.Property(ad => ad.DecisionDate)
            .IsRequired();

        builder.HasQueryFilter(ad => !ad.IsDeleted);
    }
}
