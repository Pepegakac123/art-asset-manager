using ArtAssetManager.Api.DTOs;
using FluentValidation;

namespace ArtAssetManager.Api.Validation
{
    public class CreateSavedSearchRequestValidator : AbstractValidator<CreateSavedSearchRequest>
    {
        public CreateSavedSearchRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nazwa kolekcji jest wymagana.")
                .MaximumLength(100).WithMessage("Nazwa jest zbyt długa (max 100 znaków).");

            RuleFor(x => x.Filter)
                .NotNull().WithMessage("Filtr nie może być pusty.");
        }
    }

    public class UpdateSavedSearchRequestValidator : AbstractValidator<UpdateSavedSearchRequest>
    {
        public UpdateSavedSearchRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nazwa kolekcji jest wymagana.")
                .MaximumLength(100).WithMessage("Nazwa jest zbyt długa.");
            RuleFor(x => x.Filter)
                .NotNull().WithMessage("Filtr nie może być pusty.");
        }
    }
}