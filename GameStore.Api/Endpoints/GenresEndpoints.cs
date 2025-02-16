using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenresEndpoints
{
    const string GenresEndpointName = "GetGenre";

    // private static readonly List<GameSummaryDto> games = [
    //     new (1, "Street Fighter", "Fighting", 19.99M, new DateOnly(1992, 7, 15)),
    //     new (2, "Final Fantasy XIV", "Roleplaying", 59.99M, new DateOnly(2010, 9, 30)),
    //     new (3, "Fifa 23", "Sports", 69.99M, new DateOnly(2022, 9, 27))
    // ];

    public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("genres").WithParameterValidation();
        
        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) =>
            await dbContext.Genres
                .Select(genre => genre.ToDto())
                .AsNoTracking()
                .ToListAsync()
        );

        // GET /games/{id}
        group.MapGet(
            "/{id}",
            async (int id, GameStoreContext dbContext) => {
                Genre? existingGenre = await dbContext.Genres.FindAsync(id);
                return existingGenre == null
                    ? Results.NotFound()
                    : Results.Ok(existingGenre.ToDto());
            }).WithName(GenresEndpointName);

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
                        GenresEndpointName,
                        new { id = game.Id },
                        game.ToGameSummaryDto()
                    );
                }

                return Results.BadRequest();
            });

        // PUT /games/{id}
        // group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        // {
        //     var existingGame = await dbContext.Games.FindAsync(id);

        //     if (existingGame is null)
        //     {
        //         return Results.NotFound();
        //     }
            
        //     dbContext.Entry(existingGame)
        //         .CurrentValues
        //         .SetValues(updatedGame.ToEntity(id));
                
        //     await dbContext.SaveChangesAsync();

        //     return Results.NoContent();
        // });

        // DELETE /games/{id}
        group.MapDelete(
            "/{id}",
            async (int id, GameStoreContext dbContext) => {
                await dbContext.Genres
                    .Where(genre => genre.Id == id)
                    .ExecuteDeleteAsync();
                return Results.NoContent();
            }
        );

        return group;
    }
}
