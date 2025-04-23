using FluentValidation;
using HiLo.Domain;
using HiLo.Feature.Game.Create;
using HiLo.Infrastructure.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using MockQueryable.Moq;
using Moq;
using Shouldly;

namespace Hilo.UnitTests.Feature.Game.Create;

public class CreateGameRequestHandlerTests
{
    private readonly Mock<HiLoDbContext> _dbContext = new();           
    private readonly Mock<IValidator<CreateGameRequest>> _validatorMock = new(MockBehavior.Strict);

    [Fact]
    public async Task WhenInvalidRequest_ShouldReturnValidationProblem()
    {
        // Arrange
        var request = new CreateGameRequest("PlayerName");
        var validationResult = new FluentValidation.Results.ValidationResult([
            new FluentValidation.Results.ValidationFailure("PlayerName", "Player name is required.")
        ]);
        _validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        
        // Act
        var result = await CreateGameRequestHandler.Handle(
            request,
            _validatorMock.Object,
            _dbContext.Object,
            CancellationToken.None
        );
        
        // Assert
        result.ShouldNotBeNull();
        var validationProblem = result.Result.ShouldBeAssignableTo<ValidationProblem>();
        validationProblem.ProblemDetails.Errors.ShouldContainKey("PlayerName");
    }
    
    [Fact]
    public async Task WhenPlayerNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateGameRequest("PlayerName");
        var validationResult = new FluentValidation.Results.ValidationResult();
        _validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        
        _dbContext
            .Setup(m => m.Players)
            .Returns(new List<HiLo.Domain.Player>().AsQueryable().BuildMockDbSet().Object);
        
        // Act
        var result = await CreateGameRequestHandler.Handle(
            request,
            _validatorMock.Object,
            _dbContext.Object,
            CancellationToken.None
        );
        
        // Assert
        result.ShouldNotBeNull();
        var badRequest = result.Result.ShouldBeAssignableTo<BadRequest<string>>();
        badRequest.Value.ShouldBe("Player not found.");
    }

    [Fact]
    public async Task WhenValidRequest_ShouldCreateSessionAndReturnOk()
    {
        // Arrange
        const string playerName = "PlayerName";
        var request = new CreateGameRequest(playerName);
        var validationResult = new FluentValidation.Results.ValidationResult();
        _validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        List<GameSession> sessions = [];
        var sessionsMock = sessions.BuildMockDbSet();
        sessionsMock.
            Setup(m => m.Add(It.IsAny<GameSession>()))
            .Callback<GameSession>(session => sessions.Add(session));
        _dbContext
            .Setup(m => m.Players)
            .Returns(new List<HiLo.Domain.Player> {new() { Name = playerName }}.BuildMockDbSet().Object);
        _dbContext
            .Setup(m => m.Sessions)
            .Returns(sessionsMock.Object);
        
        // Act
        var result = await CreateGameRequestHandler.Handle(
            request,
            _validatorMock.Object,
            _dbContext.Object,
            CancellationToken.None
        );
        
        // Assert
        result.ShouldNotBeNull();
        var okResult = result.Result.ShouldBeAssignableTo<Ok<Guid>>();
        sessions.Count.ShouldBe(1);
        okResult.Value.ShouldBe(sessions[0].SessionId);
    }
}