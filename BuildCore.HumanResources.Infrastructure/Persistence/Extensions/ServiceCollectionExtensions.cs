using BuildCore.HumanResources.Application.Common.Interfaces;
using BuildCore.HumanResources.Domain.Interfaces;
using BuildCore.HumanResources.Infrastructure.Common;
using BuildCore.HumanResources.Infrastructure.Persistence.Interceptors;
using BuildCore.HumanResources.Infrastructure.Persistence.Repositories;
using BuildCore.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BuildCore.HumanResources.Infrastructure.Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ----------------------------------------------------
            // 1. Infrastructure services
            // ----------------------------------------------------

            // services.AddHttpContextAccessor(); olmazsa HttpContext null olur, CurrentUser sınıfında HttpContext'e erişmeye çalışırken hata alırız.
            // ervices.AddHttpContextAccessor(); olmazsa AuditInterceptor sınıfında da HttpContext'e erişmeye çalışırken hata alırız.
            services.AddHttpContextAccessor(); 

            services.AddScoped<IDateTime, SystemDateTime>();
            services.AddScoped<ICurrentUser, CurrentUser>();

            // ----------------------------------------------------
            // 2. EF Core Interceptors
            // ----------------------------------------------------

            services.AddScoped<AuditInterceptor>();
            services.AddScoped<DomainEventInterceptor>();

            // ----------------------------------------------------
            // 3. DbContext
            // ----------------------------------------------------

            services.AddDbContext<HumanResourcesDbContext>((sp, options) =>
            {
                var auditInterceptor = sp.GetRequiredService<AuditInterceptor>();
                var domainEventInterceptor = sp.GetRequiredService<DomainEventInterceptor>();

                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));

                options.AddInterceptors(
                    auditInterceptor,
                    domainEventInterceptor
                );
            });

            // ----------------------------------------------------
            // 4. Repositories
            // ----------------------------------------------------

            services.AddScoped<IUserRepository, UserRepository>();

            // ----------------------------------------------------
            // 5. Unit Of Work
            // ----------------------------------------------------

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ----------------------------------------------------
            // 6. MediatR
            // ----------------------------------------------------
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });


            return services;
        }
    }
}
