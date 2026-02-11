using BuildCore.HumanResources.Application.Interfaces;
using BuildCore.HumanResources.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildCore.HumanResources.Infrastructure.Persistence.Seed;

/// <summary>
/// Veritabanı seed işlemleri için sınıf
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Veritabanını seed eder (eğer veri yoksa)
    /// </summary>
    public static async Task SeedAsync(HumanResourcesDbContext context, IServiceProvider? serviceProvider = null)
    {
        try
        {
            // Rolleri seed et ve admin rolünü al
            var (rolesAdded, adminRole) = await SeedRolesAsync(context);
            
            // Admin kullanıcısını seed et (admin rolünü parametre olarak geç)
            var userAdded = await SeedAdminUserAsync(context, serviceProvider, adminRole);

            // Tüm değişiklikleri tek seferde kaydet
            if (rolesAdded || userAdded)
            {
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // Hata durumunda loglama yapılabilir
            throw new Exception($"Seed işlemi sırasında hata oluştu: {ex.Message}", ex);
        }
    }

    private static async Task<(bool Added, Role? AdminRole)> SeedRolesAsync(HumanResourcesDbContext context)
    {
        if (await context.Roles.AnyAsync())
        {
            // Roller zaten mevcut, admin rolünü bul ve döndür
            var existingAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "ADMIN");
            return (false, existingAdminRole);
        }

        var newAdminRole = new Role("Admin", "Sistem yöneticisi rolü - Tüm yetkilere sahiptir");
        var roles = new[]
        {
            newAdminRole,
            new Role("HR Manager", "İnsan kaynakları yöneticisi rolü"),
            new Role("HR User", "İnsan kaynakları kullanıcı rolü"),
            new Role("Employee", "Çalışan rolü"),
            new Role("Viewer", "Sadece görüntüleme yetkisi olan rol")
        };

        await context.Roles.AddRangeAsync(roles);
        return (true, newAdminRole); // Roller eklendi ve admin rolü döndürüldü
    }

    private static async Task<bool> SeedAdminUserAsync(HumanResourcesDbContext context, IServiceProvider? serviceProvider = null, Role? adminRole = null)
    {
        if (await context.Users.AnyAsync())
        {
            return false; // Kullanıcılar zaten mevcut
        }

        // Password hasher'ı al (eğer varsa)
        string? passwordHash = null;
        if (serviceProvider != null)
        {
            var passwordHasher = serviceProvider.GetService<IPasswordHasher>();
            if (passwordHasher != null)
            {
                passwordHash = passwordHasher.HashPassword("Admin123!"); // Varsayılan şifre
            }
            else
            {
                throw new InvalidOperationException("PasswordHasher servisi bulunamadı. Seed işlemi için gerekli.");
            }
        }
        else
        {
            throw new InvalidOperationException("ServiceProvider bulunamadı. Seed işlemi için gerekli.");
        }

        var adminUser = new User(
            firstName: "Admin",
            lastName: "User",
            email: "admin@buildcore.com",
            phoneNumber: "+90 555 000 0000",
            department: "IT",
            position: "System Administrator",
            hireDate: DateOnly.FromDateTime(DateTime.Now),
            passwordHash: passwordHash
        );

        await context.Users.AddAsync(adminUser);

        // Admin rolünü kullanıcıya ata (UserRole entity'sini direkt ekle)
        if (adminRole != null)
        {
            var userRole = new UserRole(adminUser.Id, adminRole.Id);
            await context.UserRoles.AddAsync(userRole);
        }
        else
        {
            throw new InvalidOperationException("Admin rolü bulunamadı. Önce rolleri seed etmelisiniz.");
        }

        return true; // Kullanıcı eklendi
    }
}
