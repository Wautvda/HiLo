using FluentValidation;

namespace HiLo.Feature.Player.CreatePlayer;

public class CreatePlayerRequestValidator : AbstractValidator<CreatePlayerRequest>
{
    public CreatePlayerRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(r => r.Name).MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");
    }
}