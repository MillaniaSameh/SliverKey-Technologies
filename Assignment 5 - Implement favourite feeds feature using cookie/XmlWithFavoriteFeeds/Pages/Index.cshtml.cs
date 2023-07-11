using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System.Xml;
using System.Text.Json;


namespace XmlWithFavoriteFeeds.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDistributedCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public List<RssItem> RssItemsList { get; private set; } = new();
    public int PageNumber { get; private set; } = 1;
    public int PageSize { get; private set; } = 5;
    public int TotalPages { get; private set; } = 1;

    public IndexModel(IHttpClientFactory httpClientFactory, IDistributedCache cache, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task OnGetAsync(int pageNumber = 1, int pageSize = 5)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        await LoadOpml();
    }

    public async Task LoadOpml()
    {
        string? jsonItems = await _cache.GetStringAsync("itemsList");
        int counter = 0;

        if (string.IsNullOrEmpty(jsonItems))
        {
            var httpClient = _httpClientFactory.CreateClient();
            string OpmlUrl = "https://blue.feedland.org/opml?screenname=dave";

            HttpResponseMessage response = await httpClient.GetAsync(OpmlUrl);
            string xmlString = await response.Content.ReadAsStringAsync();

            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);

            XmlElement? root = document.DocumentElement;
            XmlNodeList feedNodes = root.GetElementsByTagName("outline");
            List<RssItem> itemsList = new List<RssItem>();

            foreach (XmlNode feedNode in feedNodes)
            {
                string feedTitle = feedNode.Attributes["text"]?.Value ?? "";
                string feedUrl = feedNode.Attributes["xmlUrl"]?.Value ?? "";
                if (feedUrl == "") continue;

                HttpResponseMessage feedResponse = await httpClient.GetAsync(feedUrl);
                string feedXmlString = await feedResponse.Content.ReadAsStringAsync();

                XmlDocument feedDocument = new XmlDocument();
                feedDocument.LoadXml(feedXmlString);

                XmlElement? feedRoot = feedDocument.DocumentElement;
                XmlNodeList itemNodes = feedRoot.GetElementsByTagName("item");

                foreach (XmlNode itemNode in itemNodes)
                {
                    counter++;
                    RssItem RssItem = CreateRssItem(itemNode, counter, feedTitle);
                    itemsList.Add(RssItem);
                }
            }

            jsonItems = JsonSerializer.Serialize(itemsList);
            await _cache.SetStringAsync("itemsList", jsonItems, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });
            RssItemsList = itemsList;
        }
        else
        {
            RssItemsList = JsonSerializer.Deserialize<List<RssItem>>(jsonItems);
        }

        TotalPages = (int)Math.Ceiling(RssItemsList.Count / (double)PageSize);
        int skip = (PageNumber - 1) * PageSize;
        RssItemsList = RssItemsList.Skip(skip).Take(PageSize).ToList();
    }

    RssItem CreateRssItem(XmlNode itemNode, int counter, string feedTitle)
    {
        XmlNode? titleNode = itemNode.SelectSingleNode("title");
        string title = (titleNode != null) ? $"- {titleNode.InnerText}" : "";

        XmlNode? descriptionNode = itemNode.SelectSingleNode("description");
        string description = (descriptionNode != null) ? descriptionNode.InnerText : "";
        HtmlString htmlDescription = new HtmlString(description);

        XmlNode? pubDateNode = itemNode.SelectSingleNode("pubDate");
        string pubDate = (pubDateNode != null) ? pubDateNode.InnerText : "";
        string formattedDate = DateTime.Parse(pubDate).ToString("dddd, MMMM dd, yyyy");

        XmlNode? linkNode = itemNode.SelectSingleNode("link");
        string link = (linkNode != null) ? linkNode.InnerText : "";

        RssItem rssItem = new RssItem
        {
            Id = counter,
            Title = title,
            Description = htmlDescription,
            PubDate = formattedDate,
            Link = link,
            FeedTitle = feedTitle,
            IsFavorite = false
        };

        return rssItem;
    }

    public async Task<IActionResult> OnPostToggleFavrorite()
    {
        int itemId = int.Parse(Request.Form["id"]);

        UpdateCookie(itemId);
        await UpdateFavoriteFeed(itemId);

        return RedirectToPage("/Index");
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

public class RssItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public HtmlString Description { get; set; }
    public string PubDate { get; set; }
    public string Link { get; set; }
    public string FeedTitle { get; set; }
    public bool IsFavorite { get; set; }
}