using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Security;

public class BlackListTokenDocument
{
    [BsonId]
    public required string Token { get; set; }
    public DateTime ExpiryTime { get; set; }
}