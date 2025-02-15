using GameStore.Api.Dtos;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

const string GameEndpointName = "GetGame";

List<GameDto> games = [
  new (1, "Street Fighter", "Fighting", 19.99M, new DateOnly(1992, 7, 15)),
  new (2, "Final Fantasy XIV", "Roleplaying", 59.99M, new DateOnly(2010, 9, 30)),
  new (3, "Fifa 23", "Sports", 69.99M, new DateOnly(2022, 9, 27))
];



// GET /
app.MapGet("/", () =>
{
  return "Hello World!";
});

// GET /games
app.MapGet("/games", () => games);

// GET /games/{id}
app.MapGet(
  "/games/{id}",
  (int id) => games.Find(g => g.Id == id)
).WithName(GameEndpointName);

// POST /games
app.MapPost("games", (CreateGameDto newGame) =>
{
  int id = games.Select(g => g.Id).Max() + 1;
  GameDto game = new(id, newGame.Name, newGame.Genre, newGame.Price, newGame.ReleaseDate);
  games.Add(game);

  return Results.CreatedAtRoute(GameEndpointName, new { id = game.Id }, game);
});
// PUT /games/{id}
app.MapPut("games/{id}", (int id, UpdateGameDto updatedGame) =>
{
  var existingGameIndex = games.FindIndex(g => g.Id == id);
  if (existingGameIndex >= 0)
  {
    games[existingGameIndex] = new GameDto(
      id,
      updatedGame.Name,
      updatedGame.Genre,
      updatedGame.Price,
      updatedGame.ReleaseDate
    );

    return Results.NoContent();
  }
  else
  {
    return Results.NotFound();
  }
});

app.Run();
