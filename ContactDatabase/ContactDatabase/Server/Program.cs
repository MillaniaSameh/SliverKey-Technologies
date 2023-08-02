using Microsoft.AspNetCore.ResponseCompression;
using EdgeDB;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ContactDatabase.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddEdgeDB(EdgeDBConnection.FromInstanceName("authcontactdb"), config =>
{
    config.SchemaNamingStrategy = INamingStrategy.SnakeCaseNamingStrategy;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.MapPost("/login", async (EdgeDBClient client, HttpContext httpContext) =>
{
    var requestBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
    var loginInput = JsonSerializer.Deserialize<LoginInput>(requestBody);

    if (string.IsNullOrEmpty(loginInput?.Username) || string.IsNullOrEmpty(loginInput?.Password))
    {
        return Results.BadRequest();
    }

    var query = $@"SELECT Contact {{ username, password, contact_role }} FILTER .username = <str>$username;";
    var contacts = await client.QueryAsync<Contact>(query, new Dictionary<string, object?>
    {
        { "username", loginInput?.Username }
    });

    if (contacts.Count > 0)
    {
        var passwordHasher = new PasswordHasher<string>();
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(null, contacts.First()?.Password ?? string.Empty, loginInput?.Password ?? string.Empty);

        if (passwordVerificationResult == PasswordVerificationResult.Success)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginInput?.Username ?? string.Empty),
                new Claim(ClaimTypes.Role, contacts.First()?.ContactRole ?? string.Empty)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );

            return Results.Ok();
        }
    }

    return Results.Unauthorized();
});

app.MapGet("/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

    return Results.Ok();
});

app.MapPost("/add-contact", async (EdgeDBClient client, HttpContext httpContext) =>
{
    var requestBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
    var newContact = JsonSerializer.Deserialize<Contact>(requestBody);

    if (newContact != null)
    {
        if (string.IsNullOrEmpty(newContact.Username)
        || string.IsNullOrEmpty(newContact.Password)
        || string.IsNullOrEmpty(newContact.FirstName)
        || string.IsNullOrEmpty(newContact.LastName)
        || string.IsNullOrEmpty(newContact.Email)
        || string.IsNullOrEmpty(newContact.Title)
        || string.IsNullOrEmpty(newContact.Description)
        || string.IsNullOrEmpty(newContact.BirthDate))
        {
            return Results.BadRequest();
        }

        var query = "INSERT Contact {username := <str>$username, password := <str>$password, contact_role := <str>$contact_role, first_name := <str>$first_name, last_name := <str>$last_name, email := <str>$email, title := <str>$title, description := <str>$description, birth_date := <str>$birth_date, marital_status := <bool>$marital_status}";
        var passwordHasher = new PasswordHasher<string>();
        string hashedPassword = passwordHasher.HashPassword(null, newContact.Password);

        await client.ExecuteAsync(query, new Dictionary<string, object?>
        {
            {"username", newContact.Username},
            {"password", hashedPassword},
            {"contact_role", newContact.ContactRole},
            {"first_name", newContact.FirstName},
            {"last_name", newContact.LastName},
            {"email", newContact.Email},
            {"title", newContact.Title},
            {"description", newContact.Description},
            {"birth_date", newContact.BirthDate},
            {"marital_status", newContact.MaritalStatus}
        });

        return Results.Ok();
    }

    return Results.BadRequest();
});

app.MapPost("/edit-contact", async (EdgeDBClient client, HttpContext httpContext) =>
{
    var requestBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
    var contact = JsonSerializer.Deserialize<Contact>(requestBody);

    if (contact != null)
    {
        var query = "UPDATE Contact FILTER .username = <str>$username AND .password = <str>$password SET {first_name := <str>$first_name, last_name := <str>$last_name, email := <str>$email, title := <str>$title, description := <str>$description, birth_date := <str>$birth_date, marital_status := <bool>$marital_status}";
        await client.ExecuteAsync(query, new Dictionary<string, object?>
        {
            {"username", contact.Username},
            {"password", contact.Password},
            {"first_name", contact.FirstName},
            {"last_name", contact.LastName},
            {"email", contact.Email},
            {"title", contact.Title},
            {"description", contact.Description},
            {"birth_date", contact.BirthDate},
            {"marital_status", contact.MaritalStatus}
        });

        return Results.Ok();
    }

    return Results.BadRequest();
});

app.MapGet("/fetch-contacts", async (EdgeDBClient client, HttpContext httpContext) =>
{
    var contacts = await client.QueryAsync<Contact>("SELECT Contact {*};");

    List<Contact> ContactsList = new();

    foreach (var contact in contacts)
    {
        if (contact != null)
            ContactsList.Add(contact);
    }

    return Results.Ok(ContactsList);
});

app.Run();
