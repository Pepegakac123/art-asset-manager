using ArtAssetManager.Api.DTOs;
using FluentValidation;

namespace ArtAssetManager.Api.Validation
{
    public class BulkUpdateAssetTagsRequestValidator : FluentValidation.AbstractValidator<BulkUpdateAssetTagsRequest>
    {

        public BulkUpdateAssetTagsRequestValidator()
        {
            RuleFor(x => x.AssetIds).NotEmpty().WithMessage("Lista assetow nie moze byc pusta");
            RuleFor(x => x.AssetIds).ForEach(rule => rule.GreaterThan(0).WithMessage("Id assetu musi byc wieksze od 0"));
            RuleFor(x => x.TagNames).NotEmpty().WithMessage("Lista tagow nie moze byc pusta");
            RuleFor(x => x.TagNames).ForEach(rule =>
            {
                rule.NotEmpty().WithMessage("Nazwa tagu nie moze byc pusta");
                rule.Length(2, 50).WithMessage("Nazwa tagu musi miec od 2 do 50 znakow");
            });
        }
    }
}