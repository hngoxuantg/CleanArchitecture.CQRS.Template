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
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(50).WithMessage("Password cannot exceed 50 characters")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit");
        }
    }
}
