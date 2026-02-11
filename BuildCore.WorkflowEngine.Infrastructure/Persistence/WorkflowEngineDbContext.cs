using BuildCore.WorkflowEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuildCore.WorkflowEngine.Infrastructure.Persistence;

/// <summary>
/// WorkflowEngine modülü için DbContext
/// </summary>
public class WorkflowEngineDbContext : DbContext
{
    public WorkflowEngineDbContext(DbContextOptions<WorkflowEngineDbContext> options) : base(options)
    {
    }

    public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; } = null!;
    public DbSet<WorkflowStep> WorkflowSteps { get; set; } = null!;
    public DbSet<Transition> Transitions { get; set; } = null!;
    public DbSet<WorkflowInstance> WorkflowInstances { get; set; } = null!;
    public DbSet<WorkflowInstanceHistory> WorkflowInstanceHistories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkflowEngineDbContext).Assembly);
    }
}
