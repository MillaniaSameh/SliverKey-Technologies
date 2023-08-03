using System.Text.Json.Serialization;

namespace ContactDatabaseHosted.Shared;

public class Contact
{
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = "";

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = "";

    [JsonPropertyName("email")]
    public string Email { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "Mr.";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("birth_date")]
    public DateTime BirthDate { get; set; } = DateTime.Now;

    [JsonPropertyName("marital_status")]
    public bool MaritalStatus { get; set; } = false;
}