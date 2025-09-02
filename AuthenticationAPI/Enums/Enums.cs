using System.Text.Json.Serialization;

namespace AuthenticationAPI.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoleType
{
    User = 1,
    Admin = 2
}
