using FluentValidation;
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
           
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(255).WithMessage("Title must not exceed 255 characters");


            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity must be >= 0");


            RuleFor(x => x.ImagePath)
                .NotEmpty().WithMessage("ImagePath is required");

            RuleFor(x => x.Description)
                .MaximumLength(5000)
                .When(x => x.Description != null)
                .WithMessage("Description too long");

            RuleFor(x => x.DeletedAt)
                .Must((book, deletedAt) =>
                {
                    if (book.IsDeleted)
                        return deletedAt != null;

                    return true;
                })
                .WithMessage("DeletedAt must have value when IsDeleted = true");
        }
    }
}