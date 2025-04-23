using HiLo.Feature.Game.List;
using HiLo.Infrastructure.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using MockQueryable.Moq;
using Moq;
using Shouldly;

namespace Hilo.UnitTests.Feature.Game.List;

public class ListGameRequestHandlerTest
{
    private readonly Mock<HiLoDbContext> _dbContext = new();
    
    [Fact]
    public async Task WhenNoSession_ShouldReturnEmptyList()
    {
        // Arrange
        _dbContext
            .Setup(m => m.Sessions)
            .Returns(new List<HiLo.Domain.GameSession>().AsQueryable().BuildMockDbSet().Object);
        
        // Act
        var result = await ListGameRequestHandler.Handle(_dbContext.Object, CancellationToken.None);
        
        // Assert
        result.ShouldNotBeNull();
        var okResult = result.ShouldBeAssignableTo<Ok<GameSessionDto[]>>();
        okResult.Value.ShouldBeEmpty();
    }
    
    [Fact]
    public async Task WhenSessionsExist_ShouldReturnList()
    {
        // Arrange
        var session1 = HiLo.Domain.GameSession.Create(1, 2);
        var session2 = HiLo.Domain.GameSession.Create(3, 4);
        
        _dbContext
            .Setup(m => m.Sessions)
            .Returns(new List<HiLo.Domain.GameSession> { session1, session2 }.AsQueryable().BuildMockDbSet().Object);
        
        // Act
        var result = await ListGameRequestHandler.Handle(_dbContext.Object, CancellationToken.None);
        
        // Assert
        result.ShouldNotBeNull();
        var okResult = result.ShouldBeAssignableTo<Ok<GameSessionDto[]>>();
        okResult.Value.ShouldNotBeNull();
        okResult.Value.Length.ShouldBe(2);
    }
}