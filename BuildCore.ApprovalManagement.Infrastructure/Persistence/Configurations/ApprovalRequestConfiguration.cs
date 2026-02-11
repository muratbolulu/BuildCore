using BuildCore.ApprovalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildCore.ApprovalManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// ApprovalRequest entity configuration
/// </summary>
public class ApprovalRequestConfiguration : IEntityTypeConfiguration<ApprovalRequest>
{
    public void Configure(EntityTypeBuilder<ApprovalRequest> builder)
    {
        builder.ToTable("ApprovalRequests");

        builder.HasKey(ar => ar.Id);

        builder.Property(ar => ar.RequestType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ar => ar.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ar => ar.RequesterUserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ar => ar.RequestData)
            .HasMaxLength(4000);

        builder.Property(ar => ar.RequestedAt)
            .IsRequired();

        // Relationships
        builder.HasMany(ar => ar.Decisions)
            .WithOne(d => d.ApprovalRequest)
            .HasForeignKey(d => d.ApprovalRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(ar => !ar.IsDeleted);
    }
}
