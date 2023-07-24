using EdgeDB;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAntiforgery();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddEdgeDB(EdgeDBConnection.FromInstanceName("authcontactdb"), config =>
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

// app.MapPost("/login", async (HttpContext httpContext, EdgeDBClient client, IAntiforgery antiforgery) =>
// {
// });

// app.MapPost("/logout", async (HttpContext httpContext) =>
// {
// });

app.Run();
