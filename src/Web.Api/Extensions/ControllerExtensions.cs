using System.Text.Json;
using System.Text.Json.Serialization;

namespace Web.Api.Extensions;

public static class ControllerExtensions
{
    public static void AddControllersConfig(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
    }
}