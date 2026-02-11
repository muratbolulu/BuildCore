using BuildCore.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationEntity = BuildCore.Notification.Domain.Entities.Notification;

namespace BuildCore.Notification.Infrastructure.Persistence.Configurations;

/// <summary>
/// Notification entity configuration
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.NotificationType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(n => n.RecipientUserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(n => n.RecipientEmail)
            .HasMaxLength(200);

        builder.Property(n => n.RecipientPhone)
            .HasMaxLength(50);

        builder.Property(n => n.Subject)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(n => n.Body)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(n => n.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(n => n.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(n => n.RetryCount)
            .IsRequired();

        builder.HasQueryFilter(n => !n.IsDeleted);
    }
}
