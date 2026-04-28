using BookStoreManagement.API.Models.Invoice;
using FluentValidation;

namespace BookStoreManagement.API.Validators
{
    public class InvoiceDetailCreateValidator : AbstractValidator<InvoiceDetailCreateDto>
    {
        public InvoiceDetailCreateValidator()
        {
            RuleFor(x => x.BookId)
                .GreaterThan(0).WithMessage("Book ID must be valid.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}
