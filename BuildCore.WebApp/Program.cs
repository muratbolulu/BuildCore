using BuildCore.HumanResources.Application.Interfaces;
using BuildCore.HumanResources.Application.UseCases;
using BuildCore.HumanResources.Infrastructure.Authentication;
using BuildCore.HumanResources.Infrastructure.Persistence;
using BuildCore.HumanResources.Infrastructure.Persistence.Extensions;
using BuildCore.HumanResources.Infrastructure.Persistence.Seed;
using BuildCore.WorkflowEngine.Application.Interfaces;
using BuildCore.WorkflowEngine.Infrastructure.Persistence;
using BuildCore.WorkflowEngine.Infrastructure.Persistence.Extensions;
using BuildCore.ApprovalManagement.Application.Interfaces;
using BuildCore.ApprovalManagement.Infrastructure.Persistence;
using BuildCore.ApprovalManagement.Infrastructure.Persistence.Extensions;
using BuildCore.Notification.Application.Interfaces;
using BuildCore.Notification.Infrastructure.Persistence;
using BuildCore.Notification.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Infrastructure ve Application servislerini ekle (DbContext ve interceptor'ları içerir)
builder.Services.AddPersistence(builder.Configuration);

// WorkflowEngine persistence servislerini ekle
builder.Services.AddWorkflowEnginePersistence(builder.Configuration);

// ApprovalManagement persistence servislerini ekle
builder.Services.AddApprovalManagementPersistence(builder.Configuration);

// Notification persistence servislerini ekle
builder.Services.AddNotificationPersistence(builder.Configuration);

// Authentication servislerini ekle
builder.Services.AddAuthenticationServices(builder.Configuration);

// Application servislerini ekle
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Session yapılandırması
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Veritabanını migrate et ve seed işlemini yap
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<HumanResourcesDbContext>();
        logger.LogInformation("Veritabanı migration başlatılıyor...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Veritabanı migration tamamlandı.");
        
        logger.LogInformation("Seed işlemi başlatılıyor...");
        await DatabaseSeeder.SeedAsync(context, services);
        logger.LogInformation("Seed işlemi tamamlandı.");

        // WorkflowEngine DbContext migration
        try
        {
            var workflowContext = services.GetRequiredService<WorkflowEngineDbContext>();
            logger.LogInformation("WorkflowEngine veritabanı migration başlatılıyor...");
            await workflowContext.Database.MigrateAsync();
            logger.LogInformation("WorkflowEngine veritabanı migration tamamlandı.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "WorkflowEngine migration hatası (devam ediliyor)");
        }

        // ApprovalManagement DbContext migration
        try
        {
            var approvalContext = services.GetRequiredService<ApprovalManagementDbContext>();
            logger.LogInformation("ApprovalManagement veritabanı migration başlatılıyor...");
            await approvalContext.Database.MigrateAsync();
            logger.LogInformation("ApprovalManagement veritabanı migration tamamlandı.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "ApprovalManagement migration hatası (devam ediliyor)");
        }

        // Notification DbContext migration
        try
        {
            var notificationContext = services.GetRequiredService<NotificationDbContext>();
            logger.LogInformation("Notification veritabanı migration başlatılıyor...");
            await notificationContext.Database.MigrateAsync();
            logger.LogInformation("Notification veritabanı migration tamamlandı.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Notification migration hatası (devam ediliyor)");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Veritabanı migration veya seed işlemi sırasında bir hata oluştu.");
        throw; // Hata durumunda uygulamanın başlamasını engelle
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
