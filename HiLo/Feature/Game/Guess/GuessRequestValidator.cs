using FluentValidation;
using HiLo.Configuration;
using Microsoft.Extensions.Options;

namespace HiLo.Feature.Game.Guess;

public class GuessRequestValidator : AbstractValidator<GuessRequest>
{
    public GuessRequestValidator(IOptions<GameConfiguration> configuration)
    {
        RuleFor(r => r.Guess).GreaterThanOrEqualTo(configuration.Value.MinValue).WithMessage($"Guess must equal or greater then {configuration.Value.MinValue}.");
        RuleFor(r => r.Guess).LessThanOrEqualTo(configuration.Value.MaxValue).WithMessage($"Guess must equal or lesser then {configuration.Value.MaxValue}.");
    }
}