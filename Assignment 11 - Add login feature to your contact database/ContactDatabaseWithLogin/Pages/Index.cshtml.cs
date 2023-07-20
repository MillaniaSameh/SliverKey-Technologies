using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EdgeDB;

namespace ContactDatabaseWithLogin.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public Contact NewContact { get; set; } = new();
    public int counter { get; set; } = 1;
    private readonly EdgeDBClient _client;

    public IndexModel(EdgeDBClient client)
    {
        _client = client;
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAddContact()
    {
        if (string.IsNullOrEmpty(NewContact.FirstName)
        || string.IsNullOrEmpty(NewContact.LastName)
        || string.IsNullOrEmpty(NewContact.Email)
        || string.IsNullOrEmpty(NewContact.Title)
        || string.IsNullOrEmpty(NewContact.BirthDate))
        {
            return Page();
        }

        counter++;

        var query = "INSERT Contact {contact_id := <int64>$contact_id, first_name := <str>$first_name, last_name := <str>$last_name, email := <str>$email, title := <str>$title, description := <str>$description, birth_date := <str>$birth_date, marital_status := <bool>$marital_status}";

        await _client.ExecuteAsync(query, new Dictionary<string, object?>
        {
            {"contact_id", Convert.ToInt64(counter)},
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

public class Contact
{
    public int Id { get; set; }
    public String FirstName { get; set; }
    public String LastName { get; set; }
    public String Email { get; set; }
    public String Title { get; set; }
    public String Description { get; set; } = "";
    public String BirthDate { get; set; }
    public bool MaritalStatus { get; set; }

    public Contact()
    {

    }

    public Contact(int id, string firstName, string lastName, string email, string title, string description, string birthDate, bool maritalStatus)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Title = title;
        Description = description;
        BirthDate = birthDate;
        MaritalStatus = maritalStatus;
    }
}