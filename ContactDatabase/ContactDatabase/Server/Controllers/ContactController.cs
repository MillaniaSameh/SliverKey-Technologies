using ContactDatabase.Shared;
using EdgeDB;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;

namespace ContactDatabase.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ContactController : Controller
{
    private readonly EdgeDBClient _client;

    public ContactController(EdgeDBClient client)
    {
        _client = client;
    }

    [HttpPost("add-contact")]
    public async Task<IActionResult> AddContact()
    {
        var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
        var newContact = JsonSerializer.Deserialize<Contact>(requestBody);

        if (newContact != null)
        {
            if (string.IsNullOrEmpty(newContact.FirstName)
            || string.IsNullOrEmpty(newContact.LastName)
            || string.IsNullOrEmpty(newContact.Email)
            || string.IsNullOrEmpty(newContact.Description))
            {
                return BadRequest();
            }

            var query = "INSERT Contact {first_name := <str>$first_name, last_name := <str>$last_name, email := <str>$email, title := <str>$title, description := <str>$description, birth_date := <datetime>$birth_date, marital_status := <bool>$marital_status}";
            await _client.ExecuteAsync(query, new Dictionary<string, object?>
            {
                {"first_name", newContact.FirstName},
                {"last_name", newContact.LastName},
                {"email", newContact.Email},
                {"title", newContact.Title},
                {"description", newContact.Description},
                {"birth_date", newContact.BirthDate},
                {"marital_status", newContact.MaritalStatus}
            });

            return Ok();
        }

        return BadRequest();
    }

    [HttpPost("edit-contact")]
    public async Task<IActionResult> EditContact()
    {
        var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
        var contact = JsonSerializer.Deserialize<Contact>(requestBody);

        if (contact != null)
        {
            var query = "UPDATE Contact FILTER .email = <str>$email SET {first_name := <str>$first_name, last_name := <str>$last_name, email := <str>$email, title := <str>$title, description := <str>$description, birth_date := <datetime>$birth_date, marital_status := <bool>$marital_status}";
            await _client.ExecuteAsync(query, new Dictionary<string, object?>
            {
                {"first_name", contact.FirstName},
                {"last_name", contact.LastName},
                {"email", contact.Email},
                {"title", contact.Title},
                {"description", contact.Description},
                {"birth_date", contact.BirthDate},
                {"marital_status", contact.MaritalStatus}
            });

            return Ok();
        }

        return BadRequest();
    }

    [HttpGet("get-contact")]
    public async Task<IActionResult> GetContact([FromQuery] string email)
    {
        var contact = await _client.QueryAsync<Contact>($"SELECT Contact {{*}} FILTER .email = '{email}';");

        if (contact != null)
        {
            return Ok(contact.First());
        }

        return BadRequest();
    }

    [HttpGet("fetch-contacts")]
    public async Task<IActionResult> FetchContacts()
    {
        var contacts = await _client.QueryAsync<Contact>("SELECT Contact {*};");

        List<Contact> ContactsList = new();

        foreach (var contact in contacts)
        {
            if (contact != null)
                ContactsList.Add(contact);
        }

        return Ok(ContactsList);
    }
}
