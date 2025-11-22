using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyExtensions
{
    public static void RegisterAllTypesImplementing<T>(this IServiceCollection services)
    {
        var initialType = typeof(T);
        var availableTypes = typeof(DependencyInjection).Assembly.GetTypes();
        var repositoryTypes = availableTypes.Where(t =>
            t.IsAssignableTo(initialType) && t is { IsAbstract: false, IsInterface: false });
        
        foreach (var repositoryType in repositoryTypes)
        {
            var relatedInterfaces = repositoryType.GetInterfaces().Where(t => initialType.IsAssignableFrom(t) && t != initialType);
            foreach (var relatedInterface in relatedInterfaces)
            {
                services.Add(new ServiceDescriptor(relatedInterface, repositoryType, ServiceLifetime.Singleton));
            }
        }
    }

    public static IResult WithCookie(this IResult result, string key, string refreshToken,
        CookieOptions options)
    {
        return new ResultWithCookie(result, key, refreshToken, options);
    }
    
    private class ResultWithCookie : IResult
    {
        private readonly IResult _result;
        private readonly string _key;
        private readonly string _value;
        private readonly CookieOptions _options;

        public ResultWithCookie(IResult result, string key, string value, CookieOptions options)
        {
            _result = result;
            _key = key;
            _value = value;
            _options = options;
        }
        
        public Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.Cookies.Append(_key, _value, _options);
            return _result.ExecuteAsync(httpContext);
        }
    }
}