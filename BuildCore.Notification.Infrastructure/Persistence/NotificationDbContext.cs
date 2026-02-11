using BuildCore.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NotificationEntity = BuildCore.Notification.Domain.Entities.Notification;

namespace BuildCore.Notification.Infrastructure.Persistence;

/// <summary>
/// Notification modülü için DbContext
/// </summary>
public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<NotificationTemplate> NotificationTemplates { get; set; } = null!;
    public DbSet<NotificationEntity> Notifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationDbContext).Assembly);
    }
}
