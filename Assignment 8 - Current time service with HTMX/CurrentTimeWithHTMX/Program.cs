var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/current-time", () =>
{
    var currentTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
    return Results.Ok(currentTime);
});

app.Run();
