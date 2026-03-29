using FluentValidation;
namespace BookStoreManagement.API.Validators
{
    public class CategoryValidator : AbstractValidator<Models.Entities.Category>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name cannot be empty")
                .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");
        }
    }
}