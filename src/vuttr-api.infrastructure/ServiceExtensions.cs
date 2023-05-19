using System.Text;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using vuttr_api.domain.entities;
using vuttr_api.domain.settings;
using vuttr_api.infrastructure.mapping;
using vuttr_api.persistence.repositories;
using vuttr_api.services;
using vuttr_api.services.contracts;

namespace vuttr_api.infrastructure;

public static class ServiceExtensions
{
    public static void AddMappingProfile(this IServiceCollection services)
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
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jWTSettings!.ValidIssuer,
                ValidAudience = jWTSettings.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWTSettings.SecretKey!))
            };
        });
    }

    public static void AddMongoDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        MongoDbSettings? mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

        services.AddIdentity<User, MongoIdentityRole>()
            .AddMongoDbStores<User, MongoIdentityRole, Guid>(mongoDbSettings!.ConnectionString, mongoDbSettings.DatabaseName)
            .AddDefaultTokenProviders();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IToolRepository, ToolRepository>();
        services.AddTransient<IAuthenticationManager, AuthenticationManager>();
        services.AddTransient<IToolService, ToolService>();
        services.AddTransient<IUserService, UserService>();
    }
}