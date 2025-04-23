using Bogus;
using FluentValidation.TestHelper;
using HiLo.Feature.Player.CreatePlayer;

namespace Hilo.UnitTests.Feature.Player.CreatePlayer;

public class CreatePlayerRequestValidatorTests
{
    private readonly CreatePlayerRequestValidator _validator = new();
    
    [Fact]
    public async Task WhenEmptyName_ShouldContainValidationError()
    {
        // Arrange
        var request = new CreatePlayerRequest(string.Empty);

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
    
    [Theory]
    [InlineData(51)]
    [InlineData(100)]
    [InlineData(200)]
    public async Task WhenInValidNameLength_ShouldContainValidationError(int stringLength)
    {
        // Arrange
        var request = new Faker<CreatePlayerRequest>()
            .CustomInstantiator(f => new CreatePlayerRequest(f.Lorem.Letter(stringLength)))
            .Generate();

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    public async Task WhenValidName_ShouldNotContainValidationErrors(int stringLength)
    {
        // Arrange
        var request = new Faker<CreatePlayerRequest>()
            .CustomInstantiator(f => new CreatePlayerRequest(f.Lorem.Letter(stringLength)))
            .Generate();


        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}