using FluentValidation;
using SSO.Application.Contracts.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.UserFeature.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(p => p.Id).NotNull().GreaterThan(0).WithMessage("The {id} is required.");

            RuleFor(p => p.Mobile)
                .NotEmpty().NotNull().WithMessage("The {mobile} is required.")
                .MinimumLength(10).WithMessage("Mobile must not be less than 10 characters.")
                .MaximumLength(20).WithMessage("Mobile must not exceed 50 characters.");
            RuleFor(p => p).MustAsync(BeUniqueMobile).WithMessage("The specified mobile already exists.");


            RuleFor(p => p.FirstName)
                .NotNull().NotEmpty().WithMessage("The {firstName} is required.")
                .MaximumLength(100).WithMessage("The firstName must not exceed 100 characters.");

            RuleFor(p => p.LastName)
                .NotNull().NotEmpty().WithMessage("The {lastName} is required.")
                .MaximumLength(100).WithMessage("The lastName must not exceed 100 characters.");

            RuleFor(p => p.Gender).NotNull().WithMessage("{gender} is required.");
        }

        public async Task<bool> BeUniqueMobile(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            return await _userRepository.IsUniqueMobileAsync(command.Mobile, command.Id, cancellationToken);
        }
    }
}
