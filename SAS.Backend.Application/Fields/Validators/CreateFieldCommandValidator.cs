using FluentValidation;
using SAS.Backend.Application.Fields.Commands;

namespace SAS.Backend.Application.Fields.Validators
{
    public class CreateFieldCommandValidator : AbstractValidator<CreateFieldCommand>
    {
        public CreateFieldCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.CropType)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Area)
                .GreaterThan(0);
        }
    }
}

