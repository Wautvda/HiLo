using FluentValidation;
using HiLo.Infrastructure.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HiLo.Feature.Player.CreatePlayer;

public static class CreatePlayerRequestHandler
{
    public static async Task<Results<Ok<string>, BadRequest<string>, ValidationProblem>> Handle(
        [FromBody] CreatePlayerRequest request
        , [FromServices] IValidator<CreatePlayerRequest> validator
        , [FromServices] HiLoDbContext dbContext
        , CancellationToken cancellation
    )
    {
        var validationResult = await validator.ValidateAsync(request, cancellation);
        if (!validationResult.IsValid) 
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        
        if (dbContext.Players.Any(x => x.Name == request.Name))
            return TypedResults.BadRequest("Player with the same name already exists.");

        var player = new Domain.Player { Name = request.Name };
        dbContext.Players.Add(player);
        await dbContext.SaveChangesAsync(cancellation);

        return TypedResults.Ok(player.Name);
    }
}