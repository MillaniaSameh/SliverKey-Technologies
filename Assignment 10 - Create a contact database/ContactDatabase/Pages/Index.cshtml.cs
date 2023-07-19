using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EdgeDB;

namespace ContactDatabase.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public Contact NewContact { get; set; } = new();
    private readonly EdgeDBClient _client;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(EdgeDBClient client, ILogger<IndexModel> logger)
    {
        _client = client;
        _logger = logger;
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPost()
    {
        if (string.IsNullOrEmpty(NewContact.FirstName)
        || string.IsNullOrEmpty(NewContact.LastName)
        || string.IsNullOrEmpty(NewContact.Email)
        || string.IsNullOrEmpty(NewContact.Title)
        || string.IsNullOrEmpty(NewContact.DateOfBirth))
        {
            return Page();
        }

        // var client = new EdgeDBClient();

        // var result = await client.QueryAsync<Contact>("SELECT Person { Name, Age }");

        // using var conn = await EdgeDb.Driver.EdgeDbConnection.ConnectAsync(
        //     "edgedb://user:password@localhost:edgedb"
        // );

        // var query = $"INSERT Person {{ first_name := '{FirstName}', last_name := '{LastName}', email := '{Email}' }};";
        // await conn.Execute(query);



        var connString = EdgeDBConnection.Parse("edgedb://millania:millania@localhost:5656/ContactDatabase");
        var client = new EdgeDBClient(connString);

        await client.ExecuteAsync(
            "INSERT Contact {"
            + "id := <int64>$id,"
            + "first_name := <str>$first_name,"
            + "last_name := <str>$last_name,"
            + "email := <str>$email,"
            + "title := <str>$title,"
            + "description := <str>$description,"
            + "date_of_birth := <str>$date_of_birth,"
            + "marital_status := <bool>$marital_status"
            + "}",
            NewContact
        );

        return Page();
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
    public String DateOfBirth { get; set; }
    public bool MaritalStatus { get; set; }

    public Contact()
    {

    }

    public Contact(int id, string firstName, string lastName, string email, string title, string description, string dateOfBirth, bool maritalStatus)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Title = title;
        Description = description;
        DateOfBirth = dateOfBirth;
        MaritalStatus = maritalStatus;
    }
}