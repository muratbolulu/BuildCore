using BuildCore.ApprovalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuildCore.ApprovalManagement.Infrastructure.Persistence;

/// <summary>
/// ApprovalManagement modülü için DbContext
/// </summary>
public class ApprovalManagementDbContext : DbContext
{
    public ApprovalManagementDbContext(DbContextOptions<ApprovalManagementDbContext> options) : base(options)
    {
    }

    public DbSet<ApprovalRule> ApprovalRules { get; set; } = null!;
    public DbSet<ApprovalChain> ApprovalChains { get; set; } = null!;
    public DbSet<ApprovalRequest> ApprovalRequests { get; set; } = null!;
    public DbSet<ApprovalDecision> ApprovalDecisions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApprovalManagementDbContext).Assembly);
    }
}
