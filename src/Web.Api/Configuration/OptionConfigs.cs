using Infrastructure.Configuration;

namespace Web.Api.Configuration;

public static class OptionConfigs
{
    public static void AddOptionsConfig(this IServiceCollection services, IConfiguration configuration, ILogger logger,
        WebApplicationBuilder builder)
    {
        services.Configure<ApplicationConfiguration>(configuration.GetSection(ApplicationConfiguration.SectionName));
        services.Configure<JwtConfiguration>(configuration.GetSection(JwtConfiguration.SectionName));
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });
        
        if (builder.Environment.IsDevelopment())
        {
            services.Configure<ServiceConfig>(config =>
            {
                config.Services = new List<ServiceDescriptor>(builder.Services);

                config.Path = "/listservices";
            });
        }
        
        logger.LogInformation("{Project} were configured", "Options");
    }
}