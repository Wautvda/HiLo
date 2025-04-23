using HiLo.Infrastructure.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiLo.Feature.Game.Join;

public static class JoinGameRequestHandler
{
        public static async Task<Results<Ok, NotFound<string>>> Handle(
            [FromRoute] Guid sessionId
            , [FromHeader] string playerName
            , [FromServices] HiLoDbContext dbContext
            , CancellationToken cancellationToken
        )
        {
            var session =
                await dbContext.Sessions
                    .Include(s => s.Statistics)
                    .FirstOrDefaultAsync(x => x.SessionId == sessionId, cancellationToken: cancellationToken);
            if (session is null)
                return TypedResults.NotFound("Session not found.");
        
            var player =
                await dbContext.Players.FirstOrDefaultAsync(x => x.Name == playerName,
                    cancellationToken: cancellationToken);
            if (player is null)
                return TypedResults.NotFound("Player not found.");
        
            session.AddPlayer(player);
            await dbContext.SaveChangesAsync(cancellationToken);

            return TypedResults.Ok();
        }
}