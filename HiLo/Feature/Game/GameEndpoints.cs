using HiLo.Feature.Game.Create;

namespace HiLo.Feature.Game;

public static class GameEndpoints
{
    public static void AddGameEndpoints(this WebApplication app)
    {
        var game =
            app
                .NewVersionedApi("Game")
                .WithTags("Game");

        var gameV1 =
            game
                .MapGroup($"{PathConstants.BasePath}/game")
                .HasApiVersion(1, 0);

        gameV1
            .MapPost("create", CreateGameRequestHandler.Handle);
    }
}