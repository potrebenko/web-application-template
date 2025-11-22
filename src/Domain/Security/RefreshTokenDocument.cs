
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Security;

public class RefreshTokenDocument
{
    [BsonId]
    public required string Token { get; set; }
    public required string UserId { get; set; }
    public required string IpAddress { get; set; }
    public required string Email { get; set; }
    public DateTime ExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; }
}