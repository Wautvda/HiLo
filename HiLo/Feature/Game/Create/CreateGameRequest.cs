namespace HiLo.Feature.Game.Create;

public record CreateGameRequest(string PlayerName, int MinValue = 0, int MaxValue = 100);   