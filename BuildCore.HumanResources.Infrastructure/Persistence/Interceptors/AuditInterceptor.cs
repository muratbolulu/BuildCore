using BuildCore.HumanResources.Application.Common.Interfaces;
using BuildCore.SharedKernel.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildCore.HumanResources.Infrastructure.Persistence.Interceptors
{
    public sealed class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUser _currentUser;
        private readonly IDateTime _dateTime;

        public AuditInterceptor(
            ICurrentUser currentUser,
            IDateTime dateTime)
        {
            _currentUser = currentUser;
            _dateTime = dateTime;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            if (eventData.Context is null)
                return base.SavingChanges(eventData, result);

            UpdateAuditFields(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is null)
                return base.SavingChangesAsync(eventData, result, cancellationToken);

            UpdateAuditFields(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateAuditFields(DbContext context)
        {
            var entries = context.ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    // Seed işlemleri veya sistem işlemleri için varsayılan kullanıcı
                    var userId = _currentUser.UserId ?? "System";
                    entry.Entity.SetCreated(
                        userId,
                        _dateTime.UtcNow);
                }

                if (entry.State == EntityState.Modified)
                {
                    // Seed işlemleri veya sistem işlemleri için varsayılan kullanıcı
                    var userId = _currentUser.UserId ?? "System";
                    entry.Entity.SetUpdated(
                        userId,
                        _dateTime.UtcNow);
                }
            }
        }
    }
}
