using EdgeDB;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAntiforgery();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddEdgeDB(EdgeDBConnection.FromInstanceName("contactdb"), config =>
{
    config.SchemaNamingStrategy = INamingStrategy.SnakeCaseNamingStrategy;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapPost("/login", async (HttpContext httpContext, IAntiforgery antiforgery) => 
{
    await antiforgery.ValidateRequestAsync(httpContext);

    string? username = httpContext.Request.Form["username"];
    string? password = httpContext.Request.Form["password"];

    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        // ModelState.AddModelError("LoginError", "Username and password are required.");
        httpContext.Response.Redirect("/");
    }

    if (username == "millania" && password == "millania")
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

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity)
        );

        httpContext.Response.Redirect("/");
    }
    else if (username == "sameh" && password == "sameh")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity)
        );

        httpContext.Response.Redirect("/");
    }
    else
    {
        // ModelState.AddModelError("LoginError", "Invalid username or password.");
        httpContext.Response.Redirect("/");
    }
});

app.MapPost("/logout", async (HttpContext httpContext) => 
{
    await httpContext.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme
    );

    httpContext.Response.Redirect("/");
});

app.Run();
