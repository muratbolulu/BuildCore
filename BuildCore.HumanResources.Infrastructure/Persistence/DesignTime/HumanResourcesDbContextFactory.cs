using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BuildCore.HumanResources.Infrastructure.Persistence.DesignTime;

/// <summary>
/// Migration'lar için Design-Time Factory
/// Bu sınıf sayesinde migration'lar Infrastructure projesinden bağımsız olarak çalıştırılabilir
/// </summary>
public class HumanResourcesDbContextFactory : IDesignTimeDbContextFactory<HumanResourcesDbContext>
{
    public HumanResourcesDbContext CreateDbContext(string[] args)
    {
        // Infrastructure projesinin kök dizinini bul
        // Design-time'da çalışma dizini genellikle Infrastructure projesinin bin klasörü olur
        var infrastructurePath = Path.GetDirectoryName(typeof(HumanResourcesDbContextFactory).Assembly.Location)!;
        
        // Infrastructure projesinin root dizinine çık
        var basePath = Path.GetFullPath(Path.Combine(infrastructurePath, "..", "..", "..", ".."));
        
        // appsettings.json dosyasının tam yolunu bul
        var infrastructureAppSettings = Path.Combine(basePath, "BuildCore.HumanResources.Infrastructure", "appsettings.json");
        var apiAppSettings = Path.Combine(basePath, "BuildCore.Api", "appsettings.json");
        var apiDevAppSettings = Path.Combine(basePath, "BuildCore.Api", "appsettings.Development.json");
        
        // Design-time için configuration builder
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(basePath);
        
        // Infrastructure appsettings.json dosyasını ekle (zorunlu)
        if (File.Exists(infrastructureAppSettings))
        {
            configurationBuilder.AddJsonFile(infrastructureAppSettings, optional: false, reloadOnChange: true);
        }
        
        // API appsettings dosyalarını ekle (opsiyonel)
        if (File.Exists(apiAppSettings))
        {
            configurationBuilder.AddJsonFile(apiAppSettings, optional: true, reloadOnChange: true);
        }
        
        if (File.Exists(apiDevAppSettings))
        {
            configurationBuilder.AddJsonFile(apiDevAppSettings, optional: true, reloadOnChange: true);
        }
        
        var configuration = configurationBuilder.Build();

        // DbContextOptions oluştur
        var optionsBuilder = new DbContextOptionsBuilder<HumanResourcesDbContext>();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=.\\SQLEXPRESS2022;Database=BuildCoreHumanResources;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString);

        return new HumanResourcesDbContext(optionsBuilder.Options);
    }
}
