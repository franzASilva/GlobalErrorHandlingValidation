using FluentValidation;

namespace GlobalErrorHandlingValidation.API.Commands.Requests.Validators;

public sealed class CreateDummyCommandValidator : AbstractValidator<CreateDummyCommand>
{
    public CreateDummyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull()
            .MaximumLength(20);

        RuleFor(x => x.Value)
            .GreaterThan(-1);
    }
}
