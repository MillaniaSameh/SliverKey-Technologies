using FavoriteFeedsWithHTMX.Pages;
using System.Text.Json;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAntiforgery();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();

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

app.UseAuthorization();

app.MapRazorPages();

app.MapPost("/toggleFavorite", async (HttpContext httpContext, IAntiforgery antiforgery) =>
{
    await antiforgery.ValidateRequestAsync(httpContext);

    // Update Cookie

    var id = Convert.ToInt32(httpContext.Request.Form["id"]);
    var userFavoriteFeeds = httpContext.Request.Cookies["favoriteFeeds"];
    var favorites = new List<int>();

    if (!string.IsNullOrEmpty(userFavoriteFeeds))
        favorites = JsonSerializer.Deserialize<List<int>>(userFavoriteFeeds);

    if (!favorites.Contains(id))
        favorites.Add(id);
    else
        favorites.Remove(id);

    httpContext.Response.Cookies.Append("favoriteFeeds", JsonSerializer.Serialize(favorites), new CookieOptions
    {
        Path = "/",
        IsEssential = true,
    });

    // Update Favorite Feed

    IDistributedCache cache = httpContext.RequestServices.GetService<IDistributedCache>();

    string? jsonItems = await cache.GetStringAsync("itemsList");

    if (!string.IsNullOrEmpty(jsonItems))
    {
        List<RssItem> tempRssItemsList = JsonSerializer.Deserialize<List<RssItem>>(jsonItems);
        RssItem rssItem = tempRssItemsList.Find(item => item.Id == id);

        if (rssItem != null)
        {
            rssItem.IsFavorite = !rssItem.IsFavorite;
            jsonItems = JsonSerializer.Serialize(tempRssItemsList);
            await cache.SetStringAsync("itemsList", jsonItems);
        }
    }

    // Redirect

    string refererUrl = httpContext.Request.Headers["Referer"].ToString();

    httpContext.Response.Redirect(refererUrl);
});

app.Run();