using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;


namespace FavoriteFeedsWithHTMX.Pages;

public class FavoriteFeedsModel : PageModel
{
    private readonly IDistributedCache _cache;
    public List<RssItem> RssFavoriteItemsList { get; private set; } = new();
    public int PageNumber { get; private set; } = 0;
    public int PageSize { get; private set; } = 5;
    public int TotalPages { get; private set; } = 0;
    public string PageName { get; private set; } = "FavoriteFeeds";

    public FavoriteFeedsModel(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task OnGetAsync(int pageNumber = 1, int pageSize = 5)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;

        string userFavoriteFeeds = HttpContext.Request.Cookies["favoriteFeeds"];

        if (!string.IsNullOrEmpty(userFavoriteFeeds) && userFavoriteFeeds != "[]")
        {
            int[] favoriteFeedsIds = JsonSerializer.Deserialize<int[]>(userFavoriteFeeds);

            string? jsonItems = await _cache.GetStringAsync("itemsList");
            List<RssItem> RssItemsList = new();

            if (!string.IsNullOrEmpty(jsonItems))
                RssItemsList = JsonSerializer.Deserialize<List<RssItem>>(jsonItems);

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
}