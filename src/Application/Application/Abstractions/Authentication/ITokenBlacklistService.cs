namespace Application.Abstractions.Authentication;

public interface ITokenBlacklistService
{
    /// <summary>
    /// Adds a token to the blacklist.
    /// </summary>
    /// <param name="token">The token to blacklist.</param>
    /// <param name="expiryTime">When the token expires (used for cleanup).</param>
    /// <returns>Task</returns>
    void BlacklistToken(string token, DateTime expiryTime);
    
    /// <summary>
    /// Checks if a token is blacklisted.
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <returns>True if blacklisted, false otherwise.</returns>
    bool IsTokenBlacklisted(string token);
}
