using FluentValidation;
using FluentValidation.Results;
using HiLo.Domain;
using HiLo.Feature.Game.Guess;
using HiLo.Infrastructure.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using MockQueryable.Moq;
using Moq;
using Shouldly;

namespace Hilo.UnitTests.Feature.Game.Guess;

public class GuessRequestHandlerTests
{
    private readonly Mock<IValidator<GuessRequest>> _validator = new(MockBehavior.Strict);
    private readonly Mock<HiLoDbContext> _dbContext = new();
    
    [Fact]
    public async Task WhenSessionNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var playerName = "PlayerName";
        var request = new GuessRequest(1);
        
        _dbContext
            .Setup(m => m.Sessions)
            .Returns(new List<GameSession>().AsQueryable().BuildMockDbSet().Object);
        
        // Act
        var result = await GuessRequestHandler.Handle(
            sessionId,
            playerName,
            request,
            _validator.Object,
            _dbContext.Object,
            CancellationToken.None
        );
        
        // Assert
        result.ShouldNotBeNull();
        var badRequest = result.Result.ShouldBeAssignableTo<BadRequest<string>>();
        badRequest.Value.ShouldBe("Session not found.");
    }
    
    [Fact]
    public async Task WhenPlayerNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        const string playerName = "PlayerName";
        var request = new GuessRequest(1);
        
        var session = GameSession.Create(1, 100);
        _dbContext
            .Setup(m => m.Sessions)
            .Returns(new List<GameSession> { session }.AsQueryable().BuildMockDbSet().Object);
        
        _dbContext
            .Setup(m => m.Players)
            .Returns(new List<HiLo.Domain.Player>().AsQueryable().BuildMockDbSet().Object);
        
        // Act
        var result = await GuessRequestHandler.Handle(
            session.SessionId,
            playerName,
            request,
            _validator.Object,
            _dbContext.Object,
            CancellationToken.None
        );
        
        // Assert
        result.ShouldNotBeNull();
        var badRequest = result.Result.ShouldBeAssignableTo<BadRequest<string>>();
        badRequest.Value.ShouldBe("Player not found.");
    }
    
    [Fact]
    public async Task WhenValidationFails_ShouldReturnValidationProblem()
    {
        // Arrange
        const string playerName = "PlayerName";
        var request = new GuessRequest(1);
        var validationResult = new ValidationResult(new List<ValidationFailure>{new ValidationFailure("Guess", "Guess Error")});
        _validator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        
        var session = GameSession.Create(1, 100);
        _dbContext
            .Setup(m => m.Sessions)
            .Returns(new List<GameSession> { session }.AsQueryable().BuildMockDbSet().Object);
        
        _dbContext
            .Setup(m => m.Players)
            .Returns(new List<HiLo.Domain.Player> { new() { Name = playerName } }.AsQueryable().BuildMockDbSet().Object);
        
        // Act
        var result = await GuessRequestHandler.Handle(
            session.SessionId,
            playerName,
            request,
            _validator.Object,
            _dbContext.Object,
            CancellationToken.None
        );
        
        // Assert
        result.ShouldNotBeNull();
        var validationProblem = result.Result.ShouldBeAssignableTo<ValidationProblem>();
        validationProblem.ProblemDetails.Errors.ShouldNotBeNull();
        validationProblem.ProblemDetails.Errors.ShouldContainKey("Guess");
    }
    
    [Theory]
    [InlineData(1, 1, GuessResult.Correct)]
    [InlineData(1, 2, GuessResult.Low)]
    [InlineData(1, 0, GuessResult.High)]
    public async Task WhenValidRequest_ShouldValidateGuessReturnOk(int toGuessNumber, int guess, GuessResult expected)
    {
        // Arrange
        const string playerName = "PlayerName";
        var request = new GuessRequest(guess); 
        var validationResult = new ValidationResult();
        _validator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        
        var session = GameSession.Create(1, 100);
        session.MysteryNumber = toGuessNumber;
        _dbContext
            .Setup(m => m.Sessions)
            .Returns(new List<GameSession> { session }.AsQueryable().BuildMockDbSet().Object);
        
        _dbContext
            .Setup(m => m.Players)
            .Returns(new List<HiLo.Domain.Player> { new() { Name = playerName } }.AsQueryable().BuildMockDbSet().Object);
        
        // Act
        var result = await GuessRequestHandler.Handle(
            session.SessionId,
            playerName,
            request,
            _validator.Object,
            _dbContext.Object,
            CancellationToken.None
        );
        
        // Assert
        result.ShouldNotBeNull();
        var response = result.Result.ShouldBeAssignableTo<Ok<GuessResponse>>();
        response.Value.ShouldNotBeNull();
        response.Value.Result.ShouldBe(expected);
    }
}