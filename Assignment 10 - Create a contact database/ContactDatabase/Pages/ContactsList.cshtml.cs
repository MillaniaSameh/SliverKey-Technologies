using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EdgeDB;

namespace ContactDatabase.Pages;

public class ContactsListModel : PageModel
{
    public List<Contact> ContactsList { get; private set; } = new();
    private readonly EdgeDBClient _client;

    public ContactsListModel(EdgeDBClient client)
    {
        _client = client;
    }

    public async Task<IActionResult> OnGet()
    {
        var contacts = await _client.QueryAsync("SELECT Contact { contact_id, first_name, last_name, email, title, description, birth_date, marital_status };");

        foreach (dynamic contact in (IEnumerable<dynamic>)contacts)
        {
            ContactsList.Add(
                new Contact(
                    (int)contact.contact_id,
                    contact.first_name,
                    contact.last_name,
                    contact.email,
                    contact.title,
                    contact.description,
                    contact.birth_date,
                    contact.marital_status
                )
            );
        }

        return Page();
    }
}