using FluentValidation.TestHelper;
using HiLo.Configuration;
using HiLo.Feature.Game.Guess;
using Microsoft.Extensions.Options;
using Shouldly;

namespace Hilo.UnitTests.Feature.Game.Guess;

public class GuessRequestValidatorTests
{
    private readonly GuessRequestValidator _validator = new(new OptionsWrapper<GameConfiguration>(new GameConfiguration()));
    
    [Theory]
    [InlineData(-1, "Guess must equal or greater then 0.")]
    [InlineData( 101, "Guess must equal or lesser then 100.")]
    public async Task WhenInvalidGuess_ShouldReturnError(int guess, string expectedErrorMessage)
    {
        // Arrange
        var request = new GuessRequest(guess);

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBe(false);
        result.ShouldHaveValidationErrorFor(x => x.Guess);
        result.Errors.ShouldContain(x => x.ErrorMessage == expectedErrorMessage);
    }
    
    [Fact]
    public async Task WhenValidRequest_ShouldNotReturnError()
    {
        // Arrange
        var request = new GuessRequest(50);

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBe(true);
        result.ShouldNotHaveAnyValidationErrors();
    }
}