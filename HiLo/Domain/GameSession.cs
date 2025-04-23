namespace HiLo.Domain;

public class GameSession
{
    public Guid SessionId { get; init; } = Guid.NewGuid();
    public int MysteryNumber { get; set; }
    public int Min { get; set; } = 1;
    public int Max { get; set; } = 100;
    public List<Player> Players { get; set; } = [];
    public bool IsActive { get; set; } = true;
}