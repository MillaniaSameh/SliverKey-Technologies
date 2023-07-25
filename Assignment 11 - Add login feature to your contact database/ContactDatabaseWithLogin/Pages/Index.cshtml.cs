using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EdgeDB;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ContactDatabaseWithLogin.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public Contact NewContact { get; set; } = new();
    [BindProperty]
    public LoginInput LoginInput { get; set; } = new();
    private readonly EdgeDBClient _client;

    public IndexModel(EdgeDBClient client)
    {
        _client = client;
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostLogin()
    {
        if (string.IsNullOrEmpty(LoginInput.Username) || string.IsNullOrEmpty(LoginInput.Password))
        {
            ModelState.AddModelError("LoginError", "Username and password are required.");
            return Page();
        }

        var query = $@"SELECT Contact {{ username, password, contact_role }} FILTER .username = <str>$username;";
        var contacts = await _client.QueryAsync<Contact>(query, new Dictionary<string, object?>
        {
            { "username", LoginInput.Username }
        });

        if (contacts.Count > 0)
        {
            var passwordHasher = new PasswordHasher<string>();
            string hashedPassword = passwordHasher.HashPassword(null, NewContact.Password);
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(null, contacts.First()?.Password ?? string.Empty, LoginInput.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Success)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, LoginInput.Username),
                    new Claim(ClaimTypes.Role, contacts.First()?.ContactRole ?? string.Empty)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );

                return Redirect("/");
            }
        }

        ModelState.AddModelError("LoginError", "Invalid username or password.");
        return Page();
    }

    public async Task<IActionResult> OnPostLogout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return Redirect("/");
    }
}

public class LoginInput
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}