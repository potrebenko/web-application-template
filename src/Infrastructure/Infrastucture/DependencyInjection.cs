using Application.Abstractions.Authentication;
using Application.Abstractions.Database;
using Infrastructure.Authentication;
using Infrastructure.Configuration;
using Infrastructure.Database;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServices()
            .AddHealthChecks()
            .AddChecks(configuration);

        services.AddAuthentication(configuration);
        services.AddAuthorization();
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IMongoDbContext, MongoDbContext>();
        services.RegisterAllTypesImplementing<IRepository>();

        return services;
    }

    private static void AddChecks(this IHealthChecksBuilder builder, IConfiguration configuration)
    {
        builder.AddCheck("default", () => HealthCheckResult.Healthy(), tags: ["default"]);
        builder.AddCheck<CustomHealthCheck>("custom", HealthStatus.Unhealthy);
    }

    private static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfiguration = configuration.GetSection(JwtConfiguration.SectionName).Get<JwtConfiguration>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Secret)),
                    ValidIssuer = jwtConfiguration.Issuer,
                    ValidAudience = jwtConfiguration.Audience,
                    ClockSkew = TimeSpan.Zero
                };

                // Add event handler to check token blacklist
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var tokenBlacklistService =
                            context.HttpContext.RequestServices.GetRequiredService<ITokenBlacklistService>();
                        var token = context.SecurityToken as JsonWebToken;

                        if (token is not null)
                        {
                            // Check if token is blacklisted
                            bool isBlacklisted = tokenBlacklistService.IsTokenBlacklisted(token.EncodedToken);
                            if (isBlacklisted)
                            {
                                // If token is blacklisted, fail authentication
                                context.Fail("Token has been revoked");
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<IAesEncryptor, AesAesEncryptor>();

        services.AddSingleton<TokenBlacklistService>();
        services.AddSingleton<ITokenBlacklistService>(provider => provider.GetRequiredService<TokenBlacklistService>());
        services.AddHostedService(provider => provider.GetRequiredService<TokenBlacklistService>());
    }
}