using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BuildCore.WorkflowEngine.Infrastructure.Persistence.DesignTime;

/// <summary>
/// Migration oluşturma için DesignTime DbContext Factory
/// </summary>
public class WorkflowEngineDbContextFactory : IDesignTimeDbContextFactory<WorkflowEngineDbContext>
{
    public WorkflowEngineDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WorkflowEngineDbContext>();
        
        // Migration oluştururken connection string'i buradan alır
        // Gerçek connection string appsettings.json'dan gelecek
        optionsBuilder.UseSqlServer(
            "Server=.\\SQLEXPRESS2022;Database=BuildCoreWorkflowEngine;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new WorkflowEngineDbContext(optionsBuilder.Options);
    }
}
