using FluentValidation;

namespace SSO.Application.Features.AuthenticationFeature.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(p => p.UserId)
                .NotEmpty()
                .WithMessage("{UserId} is required")
                .NotNull();

            RuleFor(p => p.CurrentPassword)
              .NotEmpty().WithMessage("{CurrentPassword} is required.")
              .NotNull()
              .MaximumLength(50).WithMessage("{CurrentPassword} must not exceed 50 characters.");

            RuleFor(p => p.NewPassword)
             .NotEmpty().WithMessage("{NewPassword} is required.")
             .NotNull()
             .MaximumLength(50).WithMessage("{NewPassword} must not exceed 50 characters.");
        }
    }
}
