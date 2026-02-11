using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BuildCore.ApprovalManagement.Infrastructure.Persistence.DesignTime;

/// <summary>
/// Migration oluşturma için DesignTime DbContext Factory
/// </summary>
public class ApprovalManagementDbContextFactory : IDesignTimeDbContextFactory<ApprovalManagementDbContext>
{
    public ApprovalManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApprovalManagementDbContext>();
        
        optionsBuilder.UseSqlServer(
            "Server=.\\SQLEXPRESS2022;Database=BuildCoreApprovalManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new ApprovalManagementDbContext(optionsBuilder.Options);
    }
}
