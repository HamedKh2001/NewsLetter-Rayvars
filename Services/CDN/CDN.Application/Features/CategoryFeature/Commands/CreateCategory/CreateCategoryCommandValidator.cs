using CDN.Application.Contracts.Persistence;
using FluentValidation;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CDN.Application.Features.CategoryFeature.Commands.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryCommandValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(p => p.Title)
             .NotEmpty().WithMessage("{title} is required.")
             .NotNull()
             .MaximumLength(200).WithMessage("{title} must not exceed 200 characters.")
             .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");

            RuleFor(p => p.Path)
                .NotEmpty().WithMessage("The {path} is required.")
                .Must(BeValidPath).WithMessage("the specefic path is invalid.");
        }

        private bool BeValidPath(string path)
        {
            return Directory.Exists(path);
        }

        private async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
        {
            return await _categoryRepository.IsUniqueTitleAsync(title);
        }
    }
}
