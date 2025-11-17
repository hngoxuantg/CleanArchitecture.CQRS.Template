using FluentValidation;
using Project.Application.DTOs;

namespace Project.Application.Validators.AuthValidators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(l => l.UserName)
                .NotEmpty().WithMessage($"{nameof(LoginDto.UserName)} is required")
                .MaximumLength(50).WithMessage($"{nameof(LoginDto.UserName)} cannot exceed 50 characters");

            RuleFor(l => l.Password)
                .NotEmpty().WithMessage($"{nameof(LoginDto.Password)} is required")
                .MinimumLength(6).WithMessage($"{nameof(LoginDto.Password)} must be at least 6 characters")
                .MaximumLength(50).WithMessage("cannot exceed 50 characters");
        }
    }
}
