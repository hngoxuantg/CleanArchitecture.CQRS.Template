using FluentValidation;
using Project.Application.Features.Auth.Requests;

namespace Project.Application.Features.Auth.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(l => l.UserName)
                .NotEmpty().WithMessage($"{nameof(LoginRequest.UserName)} is required")
                .MaximumLength(50).WithMessage($"{nameof(LoginRequest.UserName)} cannot exceed 50 characters");

            RuleFor(l => l.Password)
                .NotEmpty().WithMessage($"{nameof(LoginRequest.Password)} is required")
                .MinimumLength(6).WithMessage($"{nameof(LoginRequest.Password)} must be at least 6 characters")
                .MaximumLength(50).WithMessage("cannot exceed 50 characters");
        }
    }
}
