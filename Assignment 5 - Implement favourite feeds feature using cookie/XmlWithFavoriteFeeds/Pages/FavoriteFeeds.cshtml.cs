using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;


namespace XmlWithFavoriteFeeds.Pages;

public class FavoriteFeedsModel : PageModel
{
    private readonly IDistributedCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public List<RssItem> RssFavoriteItemsList { get; private set; } = new();
    public int PageNumber { get; private set; } = 1;
    public int PageSize { get; private set; } = 5;
    public int TotalPages { get; private set; } = 1;

    public FavoriteFeedsModel(IDistributedCache cache, IHttpContextAccessor httpContextAccessor)
    {
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task OnGetAsync(int pageNumber = 1, int pageSize = 5)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;

        string userFavoriteFeeds = _httpContextAccessor.HttpContext.Request.Cookies["favoriteFeeds"];

        if (!string.IsNullOrEmpty(userFavoriteFeeds) && userFavoriteFeeds != "[]")
        {
            int[] favoriteFeedsIds = JsonSerializer.Deserialize<int[]>(userFavoriteFeeds);

            string? jsonItems = await _cache.GetStringAsync("itemsList");
            List<RssItem> RssItemsList = new();

            if (!string.IsNullOrEmpty(jsonItems))
            {
                RssItemsList = JsonSerializer.Deserialize<List<RssItem>>(jsonItems);
            }

            RssFavoriteItemsList = RssItemsList.Where(item => favoriteFeedsIds.Contains(item.Id)).ToList();

            TotalPages = (int)Math.Ceiling(RssFavoriteItemsList.Count / (double)PageSize);
            int skip = (PageNumber - 1) * PageSize;
            RssFavoriteItemsList = RssFavoriteItemsList.Skip(skip).Take(PageSize).ToList();
        }
        else
        {
            RssFavoriteItemsList = null;
        }
    }

    public async Task<IActionResult> OnPostToggleFavrorite()
    {
        int itemId = int.Parse(Request.Form["id"]);

        UpdateCookie(itemId);
        await UpdateFavoriteFeed(itemId);

        return RedirectToPage("/FavoriteFeeds");
    }

    public void UpdateCookie(int itemId)
    {
        var userFavoriteFeeds = _httpContextAccessor.HttpContext.Request.Cookies["favoriteFeeds"];
        var favorites = new List<int>();

        if (!string.IsNullOrEmpty(userFavoriteFeeds))
        {
            favorites = JsonSerializer.Deserialize<List<int>>(userFavoriteFeeds);
        }

        if (!favorites.Contains(itemId))
        {
            favorites.Add(itemId);
        }
        else
        {
            favorites.Remove(itemId);
        }

        _httpContextAccessor.HttpContext.Response.Cookies.Append("favoriteFeeds", JsonSerializer.Serialize(favorites), new CookieOptions
        {
            Path = "/",
            IsEssential = true,
        });
    }

    public async Task UpdateFavoriteFeed(int itemId)
    {
        string? jsonItems = await _cache.GetStringAsync("itemsList");

        if (!string.IsNullOrEmpty(jsonItems))
        {
            List<RssItem> tempRssItemsList = JsonSerializer.Deserialize<List<RssItem>>(jsonItems);
            RssItem rssItem = tempRssItemsList.Find(item => item.Id == itemId);

            if (rssItem != null)
            {
                rssItem.IsFavorite = !rssItem.IsFavorite;
                jsonItems = JsonSerializer.Serialize(tempRssItemsList);
                await _cache.SetStringAsync("itemsList", jsonItems);
            }
        }
    }
}