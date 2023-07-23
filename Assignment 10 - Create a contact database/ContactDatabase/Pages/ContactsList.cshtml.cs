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
        var contacts = await _client.QueryAsync<Contact>("SELECT Contact { first_name, last_name, email, title, description, birth_date, marital_status };");

        foreach (var contact in contacts)
        {
            if (contact != null)
                ContactsList.Add(contact);
        }

        return Page();
    }
}