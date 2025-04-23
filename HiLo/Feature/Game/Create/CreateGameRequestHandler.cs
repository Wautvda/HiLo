using FluentValidation;
using HiLo.Domain;
using HiLo.Infrastructure.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiLo.Feature.Game.Create;

public static class CreateGameRequestHandler
{
    public static async Task<Results<Ok<Guid>, BadRequest<string>, ValidationProblem>> Handle(
        [FromBody]CreateGameRequest request
        , [FromServices] IValidator<CreateGameRequest> validator
        , [FromServices] HiLoDbContext dbContext
        , CancellationToken cancellationToken
        )
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary()); 
        var player = await dbContext.Players.FirstOrDefaultAsync(x => x.Name == request.PlayerName, cancellationToken: cancellationToken);
        if (player is null)
            return TypedResults.BadRequest("Player not found.");
        
        var session = GameSession.Create(request.MinValue, request.MaxValue);
        session.AddPlayer(player);
        dbContext.Sessions.Add(session);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(session.SessionId);
    }
}