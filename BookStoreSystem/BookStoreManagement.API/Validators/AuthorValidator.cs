using FluentValidation;
namespace BookStoreManagement.API.Validators
{
    public class AuthorValidator : AbstractValidator<Models.Entities.Author>
    {
        public AuthorValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Author name cannot be empty")
                .MaximumLength(100).WithMessage("Author name cannot exceed 100 characters");
        }
    }
}