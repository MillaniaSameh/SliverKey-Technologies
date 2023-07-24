using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EdgeDB;

namespace ContactDatabaseWithLogin.Pages;

public class CreateContact : PageModel
{
    [BindProperty]
    public Contact NewContact { get; set; } = new();
    private readonly EdgeDBClient _client;

    public CreateContact(EdgeDBClient client)
    {
        _client = client;
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAddContact()
    {
        if (string.IsNullOrEmpty(NewContact.Username)
        || string.IsNullOrEmpty(NewContact.Password)
        || string.IsNullOrEmpty(NewContact.FirstName)
        || string.IsNullOrEmpty(NewContact.LastName)
        || string.IsNullOrEmpty(NewContact.Email)
        || string.IsNullOrEmpty(NewContact.Title)
        || string.IsNullOrEmpty(NewContact.Description)
        || string.IsNullOrEmpty(NewContact.BirthDate))
        {
            ModelState.AddModelError("ContactError", "All fields are required.");
            return Page();
        }

        var query = "INSERT Contact {username := <str>$username, password := <str>$password, contact_role := <str>$contact_role, first_name := <str>$first_name, last_name := <str>$last_name, email := <str>$email, title := <str>$title, description := <str>$description, birth_date := <str>$birth_date, marital_status := <bool>$marital_status}";
        await _client.ExecuteAsync(query, new Dictionary<string, object?>
        {
            {"username", NewContact.Username},
            {"password", NewContact.Password},
            {"contact_role", NewContact.ContactRole},
            {"first_name", NewContact.FirstName},
            {"last_name", NewContact.LastName},
            {"email", NewContact.Email},
            {"title", NewContact.Title},
            {"description", NewContact.Description},
            {"birth_date", NewContact.BirthDate},
            {"marital_status", NewContact.MaritalStatus}
        });

        return RedirectToPage("/ContactsList");
    }

    public void OnPostEdit()
    {
        string? contactJson = Request.Form["contact"];
        if (contactJson != null)
        {
            Contact? contact = System.Text.Json.JsonSerializer.Deserialize<Contact>(contactJson);
            ViewData["contact"] = contact;
        }
    }

    public async Task<IActionResult> OnPostEditContact()
    {
        var query = "UPDATE Contact FILTER .username = <str>$username AND .password = <str>$password SET {first_name := <str>$first_name, last_name := <str>$last_name, email := <str>$email, title := <str>$title, description := <str>$description, birth_date := <str>$birth_date, marital_status := <bool>$marital_status}";
        await _client.ExecuteAsync(query, new Dictionary<string, object?>
        {
            {"username", NewContact.Username},
            {"password", NewContact.Password},
            {"first_name", NewContact.FirstName},
            {"last_name", NewContact.LastName},
            {"email", NewContact.Email},
            {"title", NewContact.Title},
            {"description", NewContact.Description},
            {"birth_date", NewContact.BirthDate},
            {"marital_status", NewContact.MaritalStatus}
        });

        return RedirectToPage("/ContactsList");
    }
}