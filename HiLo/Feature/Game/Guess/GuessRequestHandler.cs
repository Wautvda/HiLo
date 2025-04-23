using FluentValidation;
using HiLo.Infrastructure.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiLo.Feature.Game.Guess;

public static  class GuessRequestHandler
{
    public static async Task<Results<Ok<GuessResponse>, BadRequest<string>, ValidationProblem>> Handle(
        [FromRoute] Guid sessionId
        , [FromHeader] string playerName
        , [FromBody] GuessRequest request
        , [FromServices] IValidator<GuessRequest> validator
        , [FromServices] HiLoDbContext dbContext
        , CancellationToken cancellationToken
    )
    {
        var session =
            await dbContext.Sessions
                .Include(s => s.Statistics)
                .FirstOrDefaultAsync(x => x.SessionId == sessionId, cancellationToken: cancellationToken);
        if (session is null)
            return TypedResults.BadRequest("Session not found.");
        
        var player =
            await dbContext.Players.FirstOrDefaultAsync(x => x.Name == playerName,
                cancellationToken: cancellationToken);
        if (player is null)
            return TypedResults.BadRequest("Player not found.");
        
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var (result, guessCount) = session.Guess(player, request.Guess);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(GuessResponse.Create(result, guessCount));
    }
}