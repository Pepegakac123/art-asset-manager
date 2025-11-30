using ArtAssetManager.Api.DTOs;
using FluentValidation;

namespace ArtAssetManager.Api.Validation
{
    public class CreateMaterialSetRequestValidator : AbstractValidator<CreateMaterialSetRequest>
    {
        public CreateMaterialSetRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Nazwa zestawu nie moze byc pusta");
            RuleFor(x => x.Name).Length(2, 50).WithMessage("Nazwa zestawu musi miec od 2 do 50 znakow");
            RuleFor(x => x.Description).Length(0, 500).WithMessage("Opis zestawu nie może być dłuższy niż 500 znakow");
        }
    }
    public class UpdateMaterialSetRequestValidator : AbstractValidator<UpdateMaterialSet>
    {
        public UpdateMaterialSetRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Nazwa zestawu nie moze byc pusta");
            RuleFor(x => x.Name).Length(2, 50).WithMessage("Nazwa zestawu musi miec od 2 do 50 znakow");
            RuleFor(x => x.Description).Length(0, 500).WithMessage("Opis zestawu nie może być dłuższy niż 500 znakow");
        }
    }
}
