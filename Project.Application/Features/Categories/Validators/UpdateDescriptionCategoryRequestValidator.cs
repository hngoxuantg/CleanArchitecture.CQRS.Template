using FluentValidation;
using Project.Application.Features.Categories.Requests;

namespace Project.Application.Features.Categories.Validators
{
    public class UpdateDescriptionCategoryRequestValidator : AbstractValidator<UpdateDescriptionCategoryRequest>
    {
        public UpdateDescriptionCategoryRequestValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description must be provided.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
