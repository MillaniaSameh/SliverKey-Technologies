using System.Text.Json.Serialization;

namespace ContactDatabaseHosted.Shared;

public class LoginInput
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = "";

    [JsonPropertyName("password")]
    public string Password { get; set; } = "";
}