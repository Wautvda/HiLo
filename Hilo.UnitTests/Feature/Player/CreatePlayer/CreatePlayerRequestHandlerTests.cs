using FluentValidation;
using HiLo.Feature.Player.CreatePlayer;
using HiLo.Infrastructure.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using MockQueryable.Moq;
using Moq;
using Shouldly;

namespace Hilo.UnitTests.Feature.Player.CreatePlayer;

public class CreatePlayerRequestHandlerTests
{
    private readonly Mock<HiLoDbContext> _dbContext = new();
    private readonly Mock<IValidator<CreatePlayerRequest>> _validatorMock = new(MockBehavior.Strict);
        
    [Fact]
    public async Task WhenInValidRequest_ShouldValidationProblem()
    {
        // Arrange
        const string playerName = "TestPlayer";
        var request = new CreatePlayerRequest(playerName);
        const string errorMessage = "Name is required.";
        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreatePlayerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult
            {
                Errors = { new FluentValidation.Results.ValidationFailure("Name", errorMessage) }
            });

        // Act
        var result = await CreatePlayerRequestHandler.Handle(request, _validatorMock.Object, _dbContext.Object, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        var validationProblem = result.Result.ShouldBeOfType<ValidationProblem>();
        validationProblem.StatusCode.ShouldBe(400);
        validationProblem.ProblemDetails.Errors.ContainsKey("Name").ShouldBeTrue();
        validationProblem.ProblemDetails.Errors["Name"].ShouldNotBeNull();
    }
        
    [Fact]
    public async Task WhenPlayerExists_ShouldReturnBadRequest()
    {
        // Arrange
        const string playerName = "TestPlayer";
        var request = new CreatePlayerRequest(playerName);
        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreatePlayerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        
        var player = new HiLo.Domain.Player { Name = playerName };
        _dbContext
            .Setup(x => x.Players)
            .Returns(new List<HiLo.Domain.Player> { player }.AsQueryable().BuildMockDbSet().Object);

        // Act
        var result = await CreatePlayerRequestHandler.Handle(request, _validatorMock.Object, _dbContext.Object, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        var badRequest = result.Result.ShouldBeOfType<BadRequest<string>>();
        badRequest.StatusCode.ShouldBe(400);
        badRequest.Value.ShouldBe("Player with the same name already exists.");
    }

    [Fact]
    public async Task WhenPlayerDoesNotExistWithValidName_ShouldAddPlayer()
    {
        // Arrange
        const string playerName = "TestPlayer";
        var request = new CreatePlayerRequest(playerName);
        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreatePlayerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        List<HiLo.Domain.Player> players = [];
        var playersDbSetMock = players.AsQueryable().BuildMockDbSet();
        playersDbSetMock
            .Setup(m => m.Add(It.IsAny<HiLo.Domain.Player>()))
            .Callback((HiLo.Domain.Player player) => players.Add(player));
        _dbContext
            .Setup(x => x.Players)
            .Returns(playersDbSetMock.Object);

        // Act
        var result = await CreatePlayerRequestHandler.Handle(request, _validatorMock.Object, _dbContext.Object, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        var badRequest = result.Result.ShouldBeOfType<Ok<string>>();
        badRequest.Value.ShouldBe(playerName);
        
        players.Count.ShouldBe(1);
        players[0].Name.ShouldBe(playerName);
    }
}