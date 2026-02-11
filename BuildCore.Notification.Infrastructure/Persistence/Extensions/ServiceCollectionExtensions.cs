using BuildCore.Notification.Application.Interfaces;
using BuildCore.Notification.Application.UseCases;
using BuildCore.Notification.Domain.Interfaces;
using BuildCore.Notification.Infrastructure.Persistence.Repositories;
using BuildCore.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildCore.Notification.Infrastructure.Persistence.Extensions;

/// <summary>
/// Notification persistence servis kayıtları
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ----------------------------------------------------
        // 1. DbContext
        // ----------------------------------------------------
        services.AddDbContext<NotificationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));
        });

        // ----------------------------------------------------
        // 2. Repositories
        // ----------------------------------------------------
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // ----------------------------------------------------
        // 3. Unit Of Work
        // ----------------------------------------------------
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ----------------------------------------------------
        // 4. Application Services
        // ----------------------------------------------------
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}
