using FluentValidation;
using SharedKernel.Extensions;
using SSO.Application.Contracts.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.UserFeature.Commands.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserCommandValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(p => p.UserName)
                .NotNull().NotEmpty().WithMessage("{UserName} is required.")
                .MaximumLength(50).WithMessage("{UserName} must not exceed 50 characters.")
                .Must(BeValidUserName).WithMessage("The userName must be Email format.")
                .MustAsync(BeUniqueUserName).WithMessage("The specified userName already exists.");

            RuleFor(p => p.Mobile)
                .NotEmpty().NotNull().WithMessage("The {mobile} is required.")
                .MinimumLength(10).WithMessage("Mobile must not be less than 10 characters.")
                .MaximumLength(20).WithMessage("Mobile must not exceed 50 characters.")
                .Must(BeValidMobile).WithMessage("Mobile not valid")
                .MustAsync(BeUniqueMobile).WithMessage("The specified mobile already exists.");


            RuleFor(p => p.Password)
                .NotNull().NotEmpty().WithMessage("{Password} is required.")
               .MaximumLength(50).WithMessage("{Password} must not exceed 50 characters.");

            RuleFor(p => p.FirstName)
                .NotNull().NotEmpty().WithMessage("The {firstName} is required.")
                .MaximumLength(100).WithMessage("The firstName must not exceed 100 characters.");

            RuleFor(p => p.LastName)
                .NotNull().NotEmpty().WithMessage("The {lastName} is required.")
                .MaximumLength(100).WithMessage("The lastName must not exceed 100 characters.");

            RuleFor(p => p.Gender).NotNull().WithMessage("The {gender} is required.");
            RuleFor(p => p.NationalType).NotNull().WithMessage("The {nationalType} is required.");

        }

        private bool BeValidUserName(string userName)
        {
            return userName.IsEmailValid();
        }

        private bool BeValidMobile(string mobile)
        {
            return mobile.IsValidMobile();
        }


        public async Task<bool> BeUniqueUserName(string userName, CancellationToken cancellationToken)
        {
            return await _userRepository.IsUniqueUserNameAsync(userName, cancellationToken);
        }

        public async Task<bool> BeUniqueMobile(string mobile, CancellationToken cancellationToken)
        {
            return await _userRepository.IsUniqueMobileAsync(mobile, cancellationToken);
        }
    }
}
