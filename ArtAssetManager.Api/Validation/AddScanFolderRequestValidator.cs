using System.Data;
using ArtAssetManager.Api.DTOs;
using FluentValidation;

namespace ArtAssetManager.Api.Validation
{
    public class AddScanFolderRequestValidator : FluentValidation.AbstractValidator<AddScanFolderRequest>
    {
        public AddScanFolderRequestValidator()
        {
            RuleFor(x => x.FolderPath).NotEmpty().WithMessage("Ścieżka do folderu nie może być pusta ");
            RuleFor(x => x.FolderPath).Must(IsValidPath).WithMessage("Ścieżka do folderu jest niedozwolona lub jest zbyt długa");
        }

        private bool IsValidPath(string path)
        {
            try
            {
                var normalizedPath = Path.GetFullPath(path);
                if (Path.GetDirectoryName(normalizedPath) == null) return false;
                return true;
            }
            catch (ArgumentException)
            {

                return false;
            }
            catch (PathTooLongException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}