namespace HiLo.Domain;

public class GameSession
{
    public Guid SessionId { get; init; }
    public int MysteryNumber { get; init; }
    public int Min { get; init; }
    public int Max { get; init; }
    public bool IsActive { get; private set; }

    private HashSet<Player> _players = [];
    public IReadOnlyCollection<Player> Players => _players.ToArray();

    private GameSession(Guid sessionId, int mysteryNumber, int min, int max, bool isActive)
    {
        SessionId = sessionId;
        MysteryNumber = mysteryNumber;
        Min = min;
        Max = max;
        IsActive = isActive;
    }

    public static GameSession Create(int min, int max)
    {
        var random = new Random();
        var mysteryNumber = random.Next(min, max);
        return new GameSession(Guid.NewGuid(), mysteryNumber, min, mysteryNumber, true);
    }

    public void AddPlayer(Player player)
    {
        _players.Add(player);
    }

    public void GameFinished()
    {
        IsActive = false;
    }
}