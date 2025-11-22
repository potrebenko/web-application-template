using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Web.Api.Endpoints;

public static class HealthChecks
{
    public static WebApplication MapCustomHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health"); // Default calls all health checks
        app.MapHealthChecks("/custom", new HealthCheckOptions
        {
            // Called by name
            Predicate = r => string.CompareOrdinal(r.Name, "custom") == 0,
            AllowCachingResponses = false,
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var response = new
                {
                    status = report.Status.ToString(),
                    duration = report.TotalDuration,
                    checks = report.Entries.Select(x =>
                        new {
                            name = x.Key,
                            status = x.Value.Status.ToString(),
                            description = x.Value.Description,
                            duration = x.Value.Duration,
                            data = x.Value.Data
                        })
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        });

        return app;
    }
}