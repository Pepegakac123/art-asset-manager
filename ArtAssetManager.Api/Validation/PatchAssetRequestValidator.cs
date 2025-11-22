using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Services;
using FluentValidation;

namespace ArtAssetManager.Api.Validation
{
    public class PatchAssetRequestValidator : AbstractValidator<PatchAssetRequest>
    {
        public PatchAssetRequestValidator()
        {

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("Nazwa pliku nie może być pusta.")
                .MaximumLength(255).WithMessage("Nazwa pliku jest zbyt długa.")
                .When(x => x.FileName != null);

            RuleFor(x => x.Rating)
                .InclusiveBetween(0, 5).WithMessage("Ocena musi być w zakresie 0-5.")
                .When(x => x.Rating.HasValue);

            RuleFor(x => x.FileType)
                .Must(BeAValidFileType).WithMessage($"Niepoprawny typ pliku. Dozwolone: {FileTypes.Image}, {FileTypes.Model}, {FileTypes.Texture}, {FileTypes.Other}")
                .When(x => x.FileType != null);
        }

        private bool BeAValidFileType(string? type)
        {
            var allowed = new[] { FileTypes.Image, FileTypes.Model, FileTypes.Texture, FileTypes.Other };
            return allowed.Contains(type);
        }
    }
}