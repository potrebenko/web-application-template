using Microsoft.AspNetCore.Http;

namespace Infrastructure.Extensions;

public static class HttpContextExtension
{
    public static string GetIpAddress(this HttpContext httpContext)
    {
        string ipAddress;

        if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            ipAddress = httpContext.Request.Headers["X-Forwarded-For"];
        }
        else
        {
            ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        }

        return ipAddress;
    }
}