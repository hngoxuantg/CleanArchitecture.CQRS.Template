using FluentValidation;
using Project.Application.Features.Categories.Request;

namespace Project.Application.Features.Categories.Validators
{
    public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage($"{nameof(CreateCategoryRequest.Name)} is required")
                .MaximumLength(100).WithMessage($"{nameof(CreateCategoryRequest.Name)} cannot exceed 100 characters");

            RuleFor(c => c.Description)
                .MaximumLength(500).WithMessage($"{nameof(CreateCategoryRequest.Description)} cannot exceed 500 characters");
        }
    }
}
