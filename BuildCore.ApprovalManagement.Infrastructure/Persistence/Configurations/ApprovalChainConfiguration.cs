using BuildCore.ApprovalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildCore.ApprovalManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// ApprovalChain entity configuration
/// </summary>
public class ApprovalChainConfiguration : IEntityTypeConfiguration<ApprovalChain>
{
    public void Configure(EntityTypeBuilder<ApprovalChain> builder)
    {
        builder.ToTable("ApprovalChains");

        builder.HasKey(ac => ac.Id);

        builder.Property(ac => ac.ApproverType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ac => ac.ApproverRole)
            .HasMaxLength(100);

        builder.Property(ac => ac.ApproverUserId)
            .HasMaxLength(100);

        builder.Property(ac => ac.Order)
            .IsRequired();

        builder.Property(ac => ac.IsRequired)
            .IsRequired();

        builder.HasQueryFilter(ac => !ac.IsDeleted);
    }
}
