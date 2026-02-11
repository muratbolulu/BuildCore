using BuildCore.SharedKernel.Interfaces;
using BuildCore.WorkflowEngine.Application.Interfaces;
using BuildCore.WorkflowEngine.Application.UseCases;
using BuildCore.WorkflowEngine.Domain.Interfaces;
using BuildCore.WorkflowEngine.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildCore.WorkflowEngine.Infrastructure.Persistence.Extensions;

/// <summary>
/// WorkflowEngine persistence servis kayıtları
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowEnginePersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ----------------------------------------------------
        // 1. DbContext
        // ----------------------------------------------------
        services.AddDbContext<WorkflowEngineDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));
        });

        // ----------------------------------------------------
        // 2. Repositories
        // ----------------------------------------------------
        services.AddScoped<IWorkflowDefinitionRepository, WorkflowDefinitionRepository>();
        services.AddScoped<IWorkflowInstanceRepository, WorkflowInstanceRepository>();

        // ----------------------------------------------------
        // 3. Unit Of Work
        // ----------------------------------------------------
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ----------------------------------------------------
        // 4. Application Services
        // ----------------------------------------------------
        services.AddScoped<IWorkflowService, WorkflowService>();

        return services;
    }
}
