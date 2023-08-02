using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ContactDatabase.Shared;

public class Contact
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = "";

    [JsonPropertyName("password")]
    public string Password { get; set; } = "";

    [JsonPropertyName("contact_role")]
    public string ContactRole { get; set; } = "Normal";

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
    public string BirthDate { get; set; } = "";

    [JsonPropertyName("marital_status")]
    public bool MaritalStatus { get; set; } = false;
}