using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
string jsonFilePath = "uploaded-images.json";


app.MapGet("/", () =>
{
    string html = $@"
    <html>
        <head>
            <meta charset=""utf-8"" />
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Image Uploader</title>
            <link href=""https://fastly.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-9ndCyUaIbzAi2FUVXJi0CjmCapSmO7SnpJef0486qhLnuZ2cdeRhO02iuK6FUUVM"" crossorigin=""anonymous"">
            <style>
                .container {{
                    font-family: Inter, system-ui, Avenir, Helvetica, Arial, sans-serif;;
                    font-size: 1rem;
                    font-weight: 400;
                    line-height: 1.5;
                    color: #212529;
                    text-align: left; 
                    margin: 30px 80px;
                    }}
            </style>
        </head>

        <body class=""container"">

            <h1>Image Uploader</h1>

            <form method=""post"" action=""/upload"" enctype=""multipart/form-data"">

                <div class=""form-group"">
                    <label class=""form-label"" for=""title"">Title</label>
                    <input class=""form-control"" type=""text"" id=""title"" name=""title"" placeholder=""Enter image title"" required>
                </div>
                <br>

                <div class=""form-group"">
                    <label class=""form-label"" for=""img"">Image</label><br>
                    <input class=""form-control"" type=""file"" id=""img"" name=""img"" accept=""image/jpeg, image/png, image/gif"" required>
                </div>
                <br>

                <button type=""submit"" class=""btn btn-primary"">Submit</button>
            </form>

        </body>
    </html>";

    return Results.Text(html, "text/html");
});


app.MapPost("/upload", async (HttpContext context) =>
{
    IFormCollection form = await context.Request.ReadFormAsync();

    string? imgTitle = form["title"];
    var imgfile = form.Files[0];

    string imgId = Guid.NewGuid().ToString();

    if (string.IsNullOrEmpty(imgTitle)) 
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Please Provide a title");
        return;
    }
    
    if (imgfile == null || (imgfile.ContentType != "image/jpeg" && imgfile.ContentType != "image/png" && imgfile.ContentType != "image/gif"))
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Only jpeg, png or gif are accepted");
        return;
    }

    using (var stream = new MemoryStream())
    {
        await imgfile.CopyToAsync(stream);

        var imgObj = new Image {
            id = imgId,
            title = imgTitle,
            img = Convert.ToBase64String(stream.ToArray())
        };

        var jsonOptions = new JsonSerializerOptions {
            WriteIndented = true,
            IncludeFields = true,
        };

        string jsonString = JsonSerializer.Serialize(imgObj, jsonOptions);

        File.WriteAllText(jsonFilePath, jsonString);
    }

    context.Response.Redirect($"/picture/{imgId}");
});


app.MapGet("/picture/{id}", async (HttpContext context) =>
{
    string jsonString = await File.ReadAllTextAsync(jsonFilePath);
    Image? imageObj = JsonSerializer.Deserialize<Image>(jsonString);

    string html = $@"
    <html>
        <head>
            <meta charset=""utf-8"" />
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Image Uploader</title>
            <link href=""https://fastly.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-9ndCyUaIbzAi2FUVXJi0CjmCapSmO7SnpJef0486qhLnuZ2cdeRhO02iuK6FUUVM"" crossorigin=""anonymous"">
            <style>
                .container {{
                    font-family: Inter, system-ui, Avenir, Helvetica, Arial, sans-serif;;
                    font-size: 1rem;
                    font-weight: 400;
                    line-height: 1.5;
                    color: #212529;
                    text-align: center; 
                    margin: 30px 80px;
                    }}
            </style>
        </head>
            <body class=""container"">
    
                <h1>{imageObj.title}</h1>
    
                <img src=""data:image/jpeg;base64,{imageObj.img}"" alt=""{imageObj.title}"" width=""500"">
                <br>
                <br>

                <a href=""/"">
                    <button type=""button"" class=""btn btn-primary"">Back</button>
                </a>
                

            </body>
        </html>";

    return Results.Text(html, "text/html");

});


app.Run();


public class Image
{
    public string id { get; set; }
    public string title { get; set; }
    public string img { get; set; }
}
