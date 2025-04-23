using FluentValidation.TestHelper;
using HiLo.Configuration;
using HiLo.Feature.Game.CreateGame;
using Microsoft.Extensions.Options;
using Shouldly;

namespace Hilo.UnitTests.Feature.Game.CreateGame;

public class CreateGameRequestValidatorTest
{
    private readonly CreateGameRequestValidator _validator = new(new OptionsWrapper<GameConfiguration>(new GameConfiguration()));
    
    [Theory]
    [InlineData("", 0, 100, "Name is required.")]
    [InlineData(" ", 0, 100, "Name is required.")]
    public async Task WhenInvalidPlayerName_ShouldReturnError(string playerName, int minValue, int maxValue, string expectedErrorMessage)
    {
        // Arrange
        var request = new CreateGameRequest(playerName, minValue, maxValue);

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBe(false);
        result.ShouldHaveValidationErrorFor(x => x.PlayerName);
        result.Errors.ShouldContain(x => x.ErrorMessage == expectedErrorMessage);
    }
    
    [Theory]
    [InlineData("Player", -1, 100, "MinValue must equal or greater then 0.")]
    [InlineData("Player", 0, 0, "MinValue must be less than MaxValue.")]
    [InlineData("Player", 10, 10, "MinValue must be less than MaxValue.")]
    public async Task WhenInvalidMinValue_ShouldReturnError(string playerName, int minValue, int maxValue, string expectedErrorMessage)
    {
        // Arrange
        var request = new CreateGameRequest(playerName, minValue, maxValue);

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBe(false);
        result.ShouldHaveValidationErrorFor(x => x.MinValue);
        result.Errors.ShouldContain(x => x.ErrorMessage == expectedErrorMessage);
    }

    [Theory]
    [InlineData("Player", 0, -1, "MaxValue must be greater than MinValue.")]
    [InlineData("Player", 10, 9, "MaxValue must be greater than MinValue.")]
    [InlineData("Player", 0, 101, "MaxValue must be equal or less than 100.")]
    public async Task WhenInvalidMaxValue_ShouldReturnError(string playerName, int minValue, int maxValue, string expectedErrorMessage)
    {
        // Arrange
        var request = new CreateGameRequest(playerName, minValue, maxValue);

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBe(false);
        result.ShouldHaveValidationErrorFor(x => x.MaxValue);
        result.Errors.ShouldContain(x => x.ErrorMessage == expectedErrorMessage);
    }
    
    [Fact]
    public async Task WhenValidRequest_ShouldNotReturnError()
    {
        // Arrange
        var request = new CreateGameRequest("Player", 1, 10);

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBe(true);
        result.ShouldNotHaveAnyValidationErrors();
    }
    
    [Fact]
    public async Task WhenValidDefaultRequest_ShouldNotReturnError()
    {
        // Arrange
        var request = new CreateGameRequest("Player");

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBe(true);
        result.ShouldNotHaveAnyValidationErrors();
    }
}