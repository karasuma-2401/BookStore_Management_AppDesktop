using BookStoreManagement.API.Models.Invoice;
using FluentValidation;

namespace BookStoreManagement.API.Validators
{
    public class InvoiceCreateValidator : AbstractValidator<InvoiceCreateDto>
    {
        public InvoiceCreateValidator()
        {
            RuleFor(x => x.Details)
                .NotEmpty().WithMessage("Cart cannot be empty.")
                .Must(x => x != null && x.Count > 0).WithMessage("You must add at least one item to the cart.");

            RuleForEach(x => x.Details).SetValidator(new InvoiceDetailCreateValidator());
        }
    }
}
