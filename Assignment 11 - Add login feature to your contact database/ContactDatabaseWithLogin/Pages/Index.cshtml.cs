using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EdgeDB;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ContactDatabaseWithLogin.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public Contact NewContact { get; set; } = new();
    private readonly EdgeDBClient _client;
    private readonly IAntiforgery _antiforgery;

    public IndexModel(EdgeDBClient client, IAntiforgery antiforgery)
    {
        _client = client;
        _antiforgery = antiforgery;
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostLogin()
    {
        await _antiforgery.ValidateRequestAsync(HttpContext);

        string? username = HttpContext.Request.Form["username"];
        string? password = HttpContext.Request.Form["password"];

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("LoginError", "Username and password are required.");
            return Page();
        }

        var query = $@"SELECT Contact {{ username, password, contact_role }} FILTER .username = <str>$username AND .password = <str>$password;";
        var contacts = await _client.QueryAsync<Contact>(query, new Dictionary<string, object?>
        {
            { "username", username },
            { "password", password }
        });

        if (contacts.Count > 0)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
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