using FluentValidation;

namespace SSO.Application.Features.AuthenticationFeature.Queries.LogoutUser
{
    internal class LogoutUserQueryValidator : AbstractValidator<LogoutUserQuery>
    {
        public LogoutUserQueryValidator()
        {
            RuleFor(p => p.AccessToken)
                .NotEmpty().WithMessage("{AccessToken} is required.")
                .NotNull();
        }
    }
}
