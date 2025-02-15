using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// GET /
app.MapGet("/", () =>
{
    return "Hello World!";
});

app.MapGamesEndpoints();

app.Run();
