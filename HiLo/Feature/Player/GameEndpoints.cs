using HiLo.Feature.Player.CreatePlayer;

namespace HiLo.Feature.Player;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this WebApplication app)
    {
        var game =
            app
                .NewVersionedApi("Player")
                .WithTags("Player");

        var gameV1 =
            game
                .MapGroup($"{PathConstants.BasePath}/player")
                .HasApiVersion(1, 0);

        gameV1
            .MapPost("create", CreatePlayerRequestHandler.Handle);
    }
}