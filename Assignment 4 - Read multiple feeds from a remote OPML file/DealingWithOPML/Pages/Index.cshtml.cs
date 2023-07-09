using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System.Xml;
using System.Text.Json;


namespace DealingWithOPML.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDistributedCache _cache;
    public List<RssItem> RssItemsList { get; private set; } = new();
    public int PageNumber { get; private set; } = 1;
    public int PageSize { get; private set; } = 20;
    public int TotalPages { get; private set; } = 1;

    public IndexModel(IHttpClientFactory httpClientFactory, IDistributedCache cache)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
    }

    public async Task OnGetAsync(int pageNumber = 1, int pageSize = 20)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        await LoadOpml();
    }

    public async Task LoadOpml()
    {
        string? jsonItems = await _cache.GetStringAsync("itemsList");

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
                    RssItem RssItem = CreateRssItem(itemNode, feedTitle);
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

    RssItem CreateRssItem(XmlNode itemNode, string feedTitle)
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
            Title = title,
            Description = htmlDescription,
            PubDate = formattedDate,
            Link = link,
            FeedTitle = feedTitle
        };

        return rssItem;
    }
}

public class RssItem
{
    public string Title { get; set; }
    public HtmlString Description { get; set; }
    public string PubDate { get; set; }
    public string Link { get; set; }
    public string FeedTitle { get; set; }
}