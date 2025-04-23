using HiLo.Feature.Game.Join;
using HiLo.Infrastructure.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using MockQueryable.Moq;
using Moq;
using Shouldly;

namespace Hilo.UnitTests.Feature.Game.Join;

public class JoinGameRequestHandlerTests
{
    private readonly Mock<HiLoDbContext> _dbContext = new();

    [Fact]
    public async Task WhenSessionNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var playerName = "Player1";

        _dbContext
            .Setup(m => m.Sessions)
            .Returns(new List<HiLo.Domain.GameSession>().AsQueryable().BuildMockDbSet().Object);

        // Act
        var result = await JoinGameRequestHandler.Handle(
            sessionId,
            playerName,
            _dbContext.Object,
            CancellationToken.None
        );

        // Assert
        result.ShouldNotBeNull();
        var notFoundResult = result.Result.ShouldBeAssignableTo<NotFound<string>>();
        notFoundResult.Value.ShouldBe("Session not found.");
    }

    [Fact]
    public async Task WhenPlayerNotFound_ShouldReturnNotFound()
    {
        // Arrange
        const string playerName = "Player1";

        var session = HiLo.Domain.GameSession.Create(1, 2);
        _dbContext
            .Setup(m => m.Sessions)
            .Returns(new List<HiLo.Domain.GameSession> { session }.AsQueryable().BuildMockDbSet().Object);

        _dbContext
            .Setup(m => m.Players)
            .Returns(new List<HiLo.Domain.Player>().AsQueryable().BuildMockDbSet().Object);

        // Act
        var result = await JoinGameRequestHandler.Handle(
            session.SessionId,
            playerName,
            _dbContext.Object,
            CancellationToken.None
        );

        // Assert
        result.ShouldNotBeNull();
        var notFoundResult = result.Result.ShouldBeAssignableTo<NotFound<string>>();
        notFoundResult.Value.ShouldBe("Player not found.");
    }

    [Fact]
    public async Task WhenValidRequest_ShouldAddPlayerToSessionAndReturnOk()
    {
        // Arrange
        const string playerName = "Player1";
        var session = HiLo.Domain.GameSession.Create(1, 2);
        var player = new HiLo.Domain.Player { Name = playerName };

        _dbContext
            .Setup(m => m.Sessions)
            .Returns(new List<HiLo.Domain.GameSession> { session }.AsQueryable().BuildMockDbSet().Object);

        _dbContext
            .Setup(m => m.Players)
            .Returns(new List<HiLo.Domain.Player> { player }.AsQueryable().BuildMockDbSet().Object);

        // Act
        var result = await JoinGameRequestHandler.Handle(
            session.SessionId,
            playerName,
            _dbContext.Object,
            CancellationToken.None
        );

        // Assert
        result.ShouldNotBeNull();
        result.Result.ShouldBeOfType<Ok>();
        session.Statistics.ShouldContain(p => p.PlayerName == playerName);
    }
}