namespace Infrastructure.Configuration;

public class JwtConfiguration
{
    public static string SectionName = "Jwt";
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpirationInMinutes { get; set; }
    
    public int RefreshTokenExpirationInMinutes { get; set; }
}