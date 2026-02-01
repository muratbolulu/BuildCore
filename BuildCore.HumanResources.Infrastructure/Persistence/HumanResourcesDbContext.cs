using BuildCore.HumanResources.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuildCore.HumanResources.Infrastructure.Persistence;

/// <summary>
/// HumanResources modülü için DbContext
/// </summary>
public class HumanResourcesDbContext : DbContext
{
    public HumanResourcesDbContext(DbContextOptions<HumanResourcesDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HumanResourcesDbContext).Assembly);
    }
}

