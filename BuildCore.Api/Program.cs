using BuildCore.Api.Authorization;
using BuildCore.HumanResources.Application.Interfaces;
using BuildCore.HumanResources.Application.UseCases;
using BuildCore.HumanResources.Infrastructure.Authentication;
using BuildCore.HumanResources.Infrastructure.Persistence;
using BuildCore.HumanResources.Infrastructure.Persistence.Extensions;
using BuildCore.HumanResources.Infrastructure.Persistence.Seed;
using BuildCore.WorkflowEngine.Infrastructure.Persistence;
using BuildCore.WorkflowEngine.Infrastructure.Persistence.Extensions;
using BuildCore.ApprovalManagement.Infrastructure.Persistence;
using BuildCore.ApprovalManagement.Infrastructure.Persistence.Extensions;
using BuildCore.Notification.Infrastructure.Persistence;
using BuildCore.Notification.Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnet/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "HumanResources API",
        Version = "v1",
        Description = "HumanResources domaini için kullanıcı CRUD işlemleri API'si"
    });

    // JWT Authentication için Swagger yapılandırması
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Örnek: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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

// JWT Authentication yapılandırması
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? "BuildCoreSecretKeyForJWTTokenGeneration2024!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BuildCore";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BuildCoreUsers";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// Authorization yapılandırması
builder.Services.AddAuthorization(options =>
{
    // Varsayılan policy'leri buraya ekleyebilirsiniz
});

// Custom authorization policy provider ve handler'ı ekle
builder.Services.AddSingleton<IAuthorizationPolicyProvider, RolePolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();

var app = builder.Build();

// Veritabanını migrate et ve seed işlemini yap
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        // HumanResources DbContext migration
        var hrContext = services.GetRequiredService<HumanResourcesDbContext>();
        logger.LogInformation("HumanResources veritabanı migration başlatılıyor...");
        await hrContext.Database.MigrateAsync();
        logger.LogInformation("HumanResources veritabanı migration tamamlandı.");
        
        logger.LogInformation("HumanResources seed işlemi başlatılıyor...");
        await DatabaseSeeder.SeedAsync(hrContext, services);
        logger.LogInformation("HumanResources seed işlemi tamamlandı.");

        // WorkflowEngine DbContext migration
        var workflowContext = services.GetRequiredService<WorkflowEngineDbContext>();
        logger.LogInformation("WorkflowEngine veritabanı migration başlatılıyor...");
        await workflowContext.Database.MigrateAsync();
        logger.LogInformation("WorkflowEngine veritabanı migration tamamlandı.");

        // ApprovalManagement DbContext migration
        var approvalContext = services.GetRequiredService<ApprovalManagementDbContext>();
        logger.LogInformation("ApprovalManagement veritabanı migration başlatılıyor...");
        await approvalContext.Database.MigrateAsync();
        logger.LogInformation("ApprovalManagement veritabanı migration tamamlandı.");

        // Notification DbContext migration
        var notificationContext = services.GetRequiredService<NotificationDbContext>();
        logger.LogInformation("Notification veritabanı migration başlatılıyor...");
        await notificationContext.Database.MigrateAsync();
        logger.LogInformation("Notification veritabanı migration tamamlandı.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Veritabanı migration veya seed işlemi sırasında bir hata oluştu.");
        throw; // Hata durumunda uygulamanın başlamasını engelle
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HumanResources API v1");
        c.RoutePrefix = "swagger"; // Swagger UI'ı /swagger adresinde açmak için
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
