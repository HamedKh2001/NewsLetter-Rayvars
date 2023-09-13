using FluentValidation;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using CDN.Application.Contracts.Persistence;

namespace CDN.Application.Features.CategoryFeature.Commands.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryCommandValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(p => p.Title)
             .NotEmpty().WithMessage("{title} is required.")
             .NotNull()
             .MaximumLength(200).WithMessage("{title} must not exceed 200 characters.");

            RuleFor(p => p).MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");

            RuleFor(p => p.Path)
                .NotEmpty().WithMessage("The {path} is required.")
                .Must(BeValidPath).WithMessage("the specefic path is invalid.");
        }

        private bool BeValidPath(string path)
        {
            return Directory.Exists(path);
        }

        private async Task<bool> BeUniqueTitle(UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            return await _categoryRepository.IsUniqueTitleAsync(command.Title, command.Id);
        }
    }
}
