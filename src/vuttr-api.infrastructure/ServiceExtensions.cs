using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using vuttr_api.domain.entities;
using vuttr_api.domain.settings;
using vuttr_api.infrastructure.mapping;
using vuttr_api.persistence;
using vuttr_api.persistence.contracts;
using vuttr_api.persistence.repositories;
using vuttr_api.services;
using vuttr_api.services.contracts;

namespace vuttr_api.infrastructure;

public static class ServiceExtensions
{
    public static void AddMappingConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
    }

    public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSettings? jWTSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();

        AuthenticationBuilder builder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jWTSettings!.ValidIssuer,
                ValidAudience = jWTSettings.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWTSettings.SecretKey!))
            };
        });
    }

    public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("vuttr-api-database");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        services.AddIdentityCore<ApplicationUser>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IToolRepository, ToolRepository>();
        services.AddTransient<IToolService, ToolService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IUserService, UserService>();
    }
}