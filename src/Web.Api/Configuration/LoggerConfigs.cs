using Serilog;

namespace Web.Api.Configuration;

public static class LoggerConfigs
{
    public static WebApplicationBuilder AddLoggerConfig(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog( (_, config) => config.ReadFrom.Configuration(builder.Configuration));
        return builder;
    }
}