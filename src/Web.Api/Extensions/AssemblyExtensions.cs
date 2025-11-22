using System.Collections.Concurrent;
using System.Reflection;

namespace Web.Api.Extensions;

public static class AssemblyExtensions
{
    private static ConcurrentDictionary<Type, string> _cachedVersions = new();
    
    public static string GetVersion(this Type type)
    {
        var assembly = Assembly.GetAssembly(type)!.GetName();
        return _cachedVersions.GetOrAdd(type, t => assembly.Version!.ToString());
    }
}