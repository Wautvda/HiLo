using FluentValidation;
using HiLo.Configuration;
using Microsoft.Extensions.Options;

namespace HiLo.Feature.Game.Create;

public class CreateGameRequestValidator : AbstractValidator<CreateGameRequest>
{
    public CreateGameRequestValidator(IOptions<GameConfiguration> configuration)
    {
        RuleFor(r => r.PlayerName).NotEmpty().WithMessage("Name is required.");
        RuleFor(r => r.MinValue).GreaterThanOrEqualTo(configuration.Value.MinValue).WithMessage($"MinValue must equal or greater then {configuration.Value.MinValue}.");
        RuleFor(r => r.MinValue).LessThan(r => r.MaxValue).WithMessage("MinValue must be less than MaxValue.");
        RuleFor(r => r.MaxValue).GreaterThan(r => r.MinValue).WithMessage("MaxValue must be greater than MinValue.");
        RuleFor(r => r.MaxValue).LessThanOrEqualTo(configuration.Value.MaxValue).WithMessage($"MaxValue must be equal or less than {configuration.Value.MaxValue}.");
    }
}
