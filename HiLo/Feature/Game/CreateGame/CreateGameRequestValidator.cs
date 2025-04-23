using FluentValidation;

namespace HiLo.Feature.Game.CreateGame;

public class CreateGameRequestValidator : AbstractValidator<CreateGameRequest>
{
    public CreateGameRequestValidator()
    {
        RuleFor(r => r.PlayerName).NotEmpty().WithMessage("Name is required.");
        RuleFor(r => r.MinValue).GreaterThanOrEqualTo(0).WithMessage("MinValue must equal or greater then 0.");
        RuleFor(r => r.MinValue).LessThan(r => r.MaxValue).WithMessage("MinValue must be less than MaxValue.");
        RuleFor(r => r.MaxValue).GreaterThanOrEqualTo(0).WithMessage("MaxValue must be greater than 0.");
        RuleFor(r => r.MaxValue).LessThanOrEqualTo(100).WithMessage("MaxValue must be equal or less than 100.");
    }
}
