var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/current-time", () =>
{
    Console.WriteLine("----------------I got here");
    var currentTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
    return Results.Ok(currentTime);
});

app.MapGet("/", async (HttpContext context) =>
{
    await context.Response.SendFileAsync("wwwroot/index.html");
});

app.Run();
