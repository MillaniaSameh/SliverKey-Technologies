using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactDatabase.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public Contact NewContact { get; set; }
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }

    public void OnPost()
    {
        _logger.LogInformation($"---------------- {NewContact.FirstName}");
        _logger.LogInformation($"---------------- {NewContact.LastName}");
        _logger.LogInformation($"---------------- {NewContact.Email}");
        _logger.LogInformation($"---------------- {NewContact.Title}");
        _logger.LogInformation($"---------------- {NewContact.Description}");
        _logger.LogInformation($"---------------- {NewContact.DateOfBirth}");
        _logger.LogInformation($"---------------- {NewContact.MaritalStatus}");
    }
}

public class Contact
{
    public int Id { get; set; }
    public String FirstName { get; set; }
    public String LastName { get; set; }
    public String Email { get; set; }
    public String Title { get; set; }
    public String Description { get; set; }
    public String DateOfBirth { get; set; }
    public bool MaritalStatus { get; set; }

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