namespace HiLo.Feature.Game.List;

public record GameSessionDto
{
    public Guid SessionId { get; set; }
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public int PlayerCount { get; set; }
    public GameSessionStatisticsDto[] Statistics { get; set; } = [];
}

public record GameSessionStatisticsDto
{
    public string PlayerName { get; set; } = null!;
    public int GuessCount { get; set; }
}