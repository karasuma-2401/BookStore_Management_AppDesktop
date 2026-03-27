using FluentValidation;
namespace BookStoreManagement.API.Validators
{
    public class BookCategoryValidator : AbstractValidator<Models.Entities.BookCategory>
    {
        public BookCategoryValidator()
        {
            RuleFor(x => x.BookId)
                .GreaterThan(0).WithMessage("BookId must be greater than 0");
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0");
        }
    }
}