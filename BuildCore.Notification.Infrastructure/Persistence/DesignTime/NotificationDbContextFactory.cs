using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BuildCore.Notification.Infrastructure.Persistence.DesignTime;

/// <summary>
/// Migration oluşturma için DesignTime DbContext Factory
/// </summary>
public class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
{
    public NotificationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NotificationDbContext>();
        
        optionsBuilder.UseSqlServer(
            "Server=.\\SQLEXPRESS2022;Database=BuildCoreNotification;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new NotificationDbContext(optionsBuilder.Options);
    }
}
