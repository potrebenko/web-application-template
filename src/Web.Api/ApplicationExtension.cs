using System.Runtime;
using Infrastructure.Configuration;
using Web.Api.Extensions;

namespace Web.Api;

public static class ApplicationExtension
{
    public static void LogApplicationInfo(this IHostApplicationBuilder builder, ILogger logger)
    {
        var version = typeof(Web.Api.Program).GetVersion();
        var options = builder.Configuration.Get<ApplicationConfiguration>();
        logger.LogInformation($"**** {options.Name} {version} ****");
        ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
        ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxCompletionPortThreads);
        logger.LogInformation("GC server mode: {IsServerGc}", GCSettings.IsServerGC);
        logger.LogInformation("GC latency mode: {LatencyMode}", GCSettings.LatencyMode);
        logger.LogInformation("GC large object heap compaction mode: {CompactionMode}",
            GCSettings.LargeObjectHeapCompactionMode);
        logger.LogInformation("Min threads: {MinWorkerThreads}", minWorkerThreads);
        logger.LogInformation("Max threads: {MaxWorkerThreads}", maxWorkerThreads);
    }
}