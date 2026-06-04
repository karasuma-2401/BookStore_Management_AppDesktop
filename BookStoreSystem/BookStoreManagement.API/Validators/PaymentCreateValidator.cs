using BookStoreManagement.API.Models.Payment;
using FluentValidation;

namespace BookStoreManagement.API.Validators
{
    public class PaymentCreateValidator : AbstractValidator<PaymentCreateDto>
    {
        public PaymentCreateValidator()
        {
            RuleFor(x => x.InvoiceId)
                .GreaterThan(0)
                .WithMessage("Invalid invoice ID.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Payment amount must be greater than 0.");
        }
    }
}