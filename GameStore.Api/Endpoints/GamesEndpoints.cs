using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GameEndpointName = "GetGame";

    // private static readonly List<GameSummaryDto> games = [
    //     new (1, "Street Fighter", "Fighting", 19.99M, new DateOnly(1992, 7, 15)),
    //     new (2, "Final Fantasy XIV", "Roleplaying", 59.99M, new DateOnly(2010, 9, 30)),
    //     new (3, "Fifa 23", "Sports", 69.99M, new DateOnly(2022, 9, 27))
    // ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();
        
        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) => await dbContext.Games
            .Include(game  => game.Genre)
            .Select(game => game.ToGameSummaryDto())
            .AsNoTracking()
            .ToListAsync()
        );

        // GET /games/{id}
        group.MapGet(
            "/{id}",
            async (int id, GameStoreContext dbContext) => {
                Game? existingGame = await dbContext.Games.FindAsync(id);
                return existingGame == null
                    ? Results.NotFound()
                    : Results.Ok(existingGame.ToGameDetailsDto());
            }).WithName(GameEndpointName);

        // POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
            {
                Game game = newGame.ToEntity();
                game.Genre = dbContext.Genres.Find(newGame.GenreId);
                await dbContext.Games.AddAsync(game);
                var result = await dbContext.SaveChangesAsync();
                if (result >= 1)
                {
                    return Results.CreatedAtRoute(
                        GameEndpointName,
                        new { id = game.Id },
                        game.ToGameSummaryDto()
                    );
                }

                return Results.BadRequest();
            });

        // PUT /games/{id}
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }
            
            dbContext.Entry(existingGame)
                .CurrentValues
                .SetValues(updatedGame.ToEntity(id));
                
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE /games/{id}
        group.MapDelete(
            "/{id}",
            async (int id, GameStoreContext dbContext) => {
                await dbContext.Games
                    .Where(game => game.Id == id)
                    .ExecuteDeleteAsync();
                return Results.NoContent();
            }
        );

        return group;
    }
}
