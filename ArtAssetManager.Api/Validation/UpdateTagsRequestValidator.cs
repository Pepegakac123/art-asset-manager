using ArtAssetManager.Api.DTOs;
using FluentValidation;

namespace ArtAssetManager.Api.Validation
{
    public class UpdateTagsRequestValidator : FluentValidation.AbstractValidator<UpdateTagsRequest>
    {

        public UpdateTagsRequestValidator()
        {
            RuleFor(x => x.TagsNames).ForEach(rule =>
            {
                rule.NotEmpty().WithMessage("Nazwa tagu nie moze byc pusta");
                rule.Length(2, 50).WithMessage("Nazwa tagu musi miec od 2 do 50 znakow");
            });
        }
    }
}