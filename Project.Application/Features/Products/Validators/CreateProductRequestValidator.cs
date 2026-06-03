using FluentValidation;
using Project.Application.Features.Products.Requests;

namespace Project.Application.Features.Products.Validators
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.CategoryId).GreaterThan(0).When(x => x.CategoryId.HasValue);
        }
    }
}
