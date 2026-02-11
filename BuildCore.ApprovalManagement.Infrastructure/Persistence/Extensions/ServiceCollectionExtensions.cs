using BuildCore.ApprovalManagement.Application.Interfaces;
using BuildCore.ApprovalManagement.Application.UseCases;
using BuildCore.ApprovalManagement.Domain.Interfaces;
using BuildCore.ApprovalManagement.Infrastructure.Persistence.Repositories;
using BuildCore.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildCore.ApprovalManagement.Infrastructure.Persistence.Extensions;

/// <summary>
/// ApprovalManagement persistence servis kayıtları
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApprovalManagementPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ----------------------------------------------------
        // 1. DbContext
        // ----------------------------------------------------
        services.AddDbContext<ApprovalManagementDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));
        });

        // ----------------------------------------------------
        // 2. Repositories
        // ----------------------------------------------------
        services.AddScoped<IApprovalRuleRepository, ApprovalRuleRepository>();
        services.AddScoped<IApprovalRequestRepository, ApprovalRequestRepository>();

        // ----------------------------------------------------
        // 3. Unit Of Work
        // ----------------------------------------------------
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ----------------------------------------------------
        // 4. Application Services
        // ----------------------------------------------------
        services.AddScoped<IApprovalService, ApprovalService>();

        return services;
    }
}
