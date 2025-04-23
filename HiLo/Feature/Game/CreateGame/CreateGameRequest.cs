namespace HiLo.Feature.Game.CreateGame;

public record CreateGameRequest(string PlayerName, int MinValue = 0, int MaxValue = 100);   