using System.Collections.Concurrent;
using Application.Abstractions.Authentication;
using Application.Abstractions.Database;
using Domain.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Authentication;

public class TokenBlacklistService(ILogger<TokenBlacklistService> logger, ITokenBlackListRepository tokenBlacklistRepository) : BackgroundService, ITokenBlacklistService
{
    private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new(StringComparer.Ordinal);
    private readonly ITokenBlackListRepository _tokenBlackListRepository = tokenBlacklistRepository;

    public void BlacklistToken(string token, DateTime expiryTime)
    {
        try
        {
            _blacklistedTokens.TryAdd(token, expiryTime);
            logger.LogInformation("Token added to blacklist");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding token to blacklist");
        }
    }

    public bool IsTokenBlacklisted(string token)
    {
        try
        {
            return _blacklistedTokens.ContainsKey(token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if token is blacklisted");
            // Default to not blacklisted if there's an error
            return false;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Token blacklist service starting");
        await LoadBlacklistAsync();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Cleanup expired tokens from blacklist every minute
                CleanupExpiredTokens();
            }
            catch (TaskCanceledException)
            {
                // This is expected when the delay is canceled due to shutdown
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during token cleanup cycle");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        
        await SaveTokenBlacklist();
        logger.LogInformation("Token blacklist service stopped");
    }

    private async Task LoadBlacklistAsync()
    {
        var tokens = await _tokenBlackListRepository.GetAllTokensAsync();
        foreach (var token in tokens)
        {
            _blacklistedTokens.TryAdd(token.Token, token.ExpiryTime);
        }
    }

    private Task SaveTokenBlacklist()
    {
        var documents = _blacklistedTokens
            .Select(x => new BlackListTokenDocument { Token = x.Key, ExpiryTime = x.Value }).ToList();
        return _tokenBlackListRepository.SaveAllTokensAsync(documents);
    }

    private void CleanupExpiredTokens()
    {
        try
        {
            var now = DateTime.UtcNow;
            var expiredTokens = _blacklistedTokens
                .Where(pair => pair.Value <= now)
                .Select(pair => pair.Key)
                .ToList();

            foreach (var token in expiredTokens)
            {
                if (_blacklistedTokens.TryRemove(token, out _))
                {
                    logger.LogDebug("Removed expired token from blacklist");
                }
            }

            if (expiredTokens.Count > 0)
            {
                logger.LogInformation("Removed {Count} expired tokens from blacklist", expiredTokens.Count);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cleaning up expired tokens");
        }
    }
}