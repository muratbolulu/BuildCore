using BuildCore.HumanResources.Application.Interfaces;
using BuildCore.HumanResources.Application.UseCases;
using BuildCore.HumanResources.Domain.Interfaces;
using BuildCore.HumanResources.Infrastructure.Persistence;
using BuildCore.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;

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
});

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=BuildCoreHumanResources;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<HumanResourcesDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repository and Service Registration
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
