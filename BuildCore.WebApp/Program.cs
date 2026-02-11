using BuildCore.HumanResources.Application.Interfaces;
using BuildCore.HumanResources.Application.UseCases;
using BuildCore.HumanResources.Infrastructure.Authentication;
using BuildCore.HumanResources.Infrastructure.Persistence;
using BuildCore.HumanResources.Infrastructure.Persistence.Extensions;
using BuildCore.HumanResources.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Infrastructure ve Application servislerini ekle (DbContext ve interceptor'ları içerir)
builder.Services.AddPersistence(builder.Configuration);

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
