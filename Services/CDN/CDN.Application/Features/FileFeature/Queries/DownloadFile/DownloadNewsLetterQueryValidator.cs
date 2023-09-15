using FluentValidation;
using System;

namespace CDN.Application.Features.FileFeature.Queries.DownloadFile
{
    public class DownloadNewsLetterQueryValidator : AbstractValidator<DownloadNewsLetterQuery>
    {
        public DownloadNewsLetterQueryValidator()
        {
            RuleFor(p => p.Id).GreaterThan(0).WithMessage("The {id} is required.");
        }
    }
}
