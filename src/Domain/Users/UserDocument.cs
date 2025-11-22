using System.Diagnostics;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Users;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class UserDocument
{
    [BsonId]
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }

    private string GetDebuggerDisplay() => ToString();
}