using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace LoginWithAuth.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public LoginInput LoginInput { get; set; }
    private readonly string _loginPath = "/Index";

    public IndexModel()
    {

    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPost()
    {
        string username = LoginInput.Username;
        string password = LoginInput.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("LoginError", "Username and password are required.");
            return Page();
        }

        if (username == "intern" && password == "summer 2023 july")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );

            return RedirectToPage();
        }
        else
        {
            ModelState.AddModelError("LoginError", "Invalid username or password.");
            return Page();
        }
    }

    public async Task<IActionResult> OnPostHandleLogout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return RedirectToPage(_loginPath);
    }
}

public class LoginInput
{
    public string Username { get; set; }
    public string Password { get; set; }
}