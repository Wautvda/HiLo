using System.ComponentModel.DataAnnotations;

namespace HiLo.Domain;

public class PlayerStatistics
{
    [MaxLength(50)]
    public string PlayerName { get; set; } = null!;
    public Player Player { get; set; } = null!;
    public Guid SessionId { get; set; }
    public GameSession Session { get; set; } = null!;
    
    public int GuessCount { get; set; }
}