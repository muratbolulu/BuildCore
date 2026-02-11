using BuildCore.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildCore.Notification.Infrastructure.Persistence.Configurations;

/// <summary>
/// NotificationTemplate entity configuration
/// </summary>
public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplates");

        builder.HasKey(nt => nt.Id);

        builder.Property(nt => nt.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(nt => nt.TemplateType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(nt => nt.Subject)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(nt => nt.Body)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(nt => nt.IsActive)
            .IsRequired();

        // Relationships
        builder.HasMany(nt => nt.Notifications)
            .WithOne(n => n.Template)
            .HasForeignKey(n => n.TemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(nt => !nt.IsDeleted);
    }
}
