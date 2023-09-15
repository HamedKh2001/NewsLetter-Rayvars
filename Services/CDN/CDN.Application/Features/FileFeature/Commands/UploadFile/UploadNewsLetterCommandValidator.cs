using CDN.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace CDN.Application.Features.FileFeature.Commands.UploadFile
{
    public class UploadNewsLetterCommandValidator : AbstractValidator<UploadNewsLettereCommand>
    {
        public UploadNewsLetterCommandValidator()
        {
            RuleFor(p => p.FormFile)
                .NotNull().WithMessage("The {formFile} is required.")
                .Must(BeValidFormFile).WithMessage("The file is invalid");

            RuleFor(p => p.TagName).Must(BeValidChars).WithMessage("The {tagName} has invalid chars.");
            RuleFor(p => p.FileName).Must(BeValidChars).WithMessage("The {fileName} has invalid chars.");
        }

        private bool BeValidChars(string input)
        {
            return input.HasInvalidFileNameChars() == false;
        }

        private bool BeValidFormFile(IFormFile formFile)
        {
            if (formFile != null)
                if (string.IsNullOrEmpty(formFile.FileName) || formFile.Length == 0)
                    return false;
            return true;
        }
    }
}
