namespace HiLo.Domain;

public class GameSession
{
    public Guid SessionId { get; init; }
    public int MysteryNumber { get; internal set; }
    public int Min { get; init; }
    public int Max { get; init; }

    private readonly HashSet<PlayerStatistics> _statistics = [];
    public IReadOnlyCollection<PlayerStatistics> Statistics => _statistics;

    private GameSession(Guid sessionId, int mysteryNumber, int min, int max)
    {
        SessionId = sessionId;
        MysteryNumber = mysteryNumber;
        Min = min;
        Max = max;
    }

    public static GameSession Create(int min, int max)
    {
        var random = new Random();
        return new GameSession(Guid.NewGuid(), random.Next(min, max), min, max);
    }

    public void AddPlayer(Player player)
    {
        if (_statistics.Any(s => s.PlayerName == player.Name)) return;
        _statistics.Add(new PlayerStatistics
        {
            PlayerName = player.Name,
            SessionId = SessionId,
            GuessCount = 0
        });
    }

    public (GuessResult, int) Guess(Player player, int number)
    {
       var count = RegisterGuess(player);

        return MysteryNumber.CompareTo(number) switch
        {
            > 0 => (GuessResult.High, count)
            , < 0 => (GuessResult.Low, count)
            , _ => (GuessResult.Correct, count)
        };
    }
    
    private int RegisterGuess(Player player)
    {
        var stat =
            _statistics
                .FirstOrDefault(s => s.PlayerName == player.Name)
            ?? new PlayerStatistics
            {
                PlayerName = player.Name,
                SessionId = SessionId,
                GuessCount = 0
            };

        _statistics.Add(stat);
        stat.GuessCount++;

        return stat.GuessCount;
    }
}