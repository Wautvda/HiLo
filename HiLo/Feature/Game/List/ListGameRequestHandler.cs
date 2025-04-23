using HiLo.Infrastructure.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiLo.Feature.Game.List;

public static class ListGameRequestHandler
{
    public static async Task<Ok<GameSessionDto[]>> Handle(
        [FromServices] HiLoDbContext dbContext
        , CancellationToken cancellationToken
    )
    {
        var sessions =
            await dbContext.Sessions
                .Include(s => s.Statistics)
                .AsNoTracking()
                .Select(s => new GameSessionDto
                {
                    SessionId = s.SessionId,
                    MinValue = s.Min,
                    MaxValue = s.Max,
                    PlayerCount = s.Statistics.Count,
                    Statistics = s.Statistics.Select(st => new GameSessionStatisticsDto
                    {
                        PlayerName = st.Player.Name,
                        GuessCount = st.GuessCount,
                    }).ToArray()
                })
                .ToArrayAsync(cancellationToken);

        return TypedResults.Ok(sessions);
    }
}