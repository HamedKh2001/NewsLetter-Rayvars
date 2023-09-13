using FluentValidation;
using System;

namespace CDN.Application.Features.FileFeature.Queries.DownloadFile
{
    public class DownloadFileQueryValidator : AbstractValidator<DownloadFileQuery>
    {
        public DownloadFileQueryValidator()
        {
            RuleFor(p => p.Id).GreaterThan(0).WithMessage("The {id} is required.");
        }
    }
}
