using GameStore.Api.Dtos;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GameEndpointName = "GetGame";

    private static readonly List<GameDto> games = [
        new (1, "Street Fighter", "Fighting", 19.99M, new DateOnly(1992, 7, 15)),
        new (2, "Final Fantasy XIV", "Roleplaying", 59.99M, new DateOnly(2010, 9, 30)),
        new (3, "Fifa 23", "Sports", 69.99M, new DateOnly(2022, 9, 27))
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();
        
        // GET /games
        group.MapGet("/", () => games);

        // GET /games/{id}
        group.MapGet(
            "/{id}",
            (int id) => {
                GameDto? existingGame = games.Find(g => g.Id == id);
                return existingGame == null ? Results.NotFound() : Results.Ok(existingGame);
            }).WithName(GameEndpointName);

        // POST /games
        group.MapPost("/", (CreateGameDto newGame) =>
            {
                int id = games.Select(g => g.Id).Max() + 1;
                GameDto game = new(id, newGame.Name, newGame.Genre, newGame.Price, newGame.ReleaseDate);
                games.Add(game);

                return Results.CreatedAtRoute(GameEndpointName, new { id = game.Id }, game);
            });

        // PUT /games/{id}
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
        {
            var existingGameIndex = games.FindIndex(g => g.Id == id);

            if (existingGameIndex == -1)
            {
                return Results.NotFound();
            }
            var existingGame = games[existingGameIndex];
            
            games[existingGameIndex] = new GameDto(
                id,
                updatedGame.Name ?? existingGame.Name,
                updatedGame.Genre ?? existingGame.Genre,
                updatedGame.Price ?? existingGame.Price,
                updatedGame.ReleaseDate ?? existingGame.ReleaseDate
            );

            return Results.NoContent();
        });

        // DELETE /games/{id}
        group.MapDelete(
            "/{id}",
            (int id) => {
                games.RemoveAll(game => game.Id == id);
                return Results.NoContent();
            }
        );

        return group;
    }
}
