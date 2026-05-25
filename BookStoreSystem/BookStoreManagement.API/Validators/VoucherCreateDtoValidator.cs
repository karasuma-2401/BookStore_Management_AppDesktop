using BookStoreManagement.API.Models.Voucher;
using FluentValidation;

namespace BookStoreManagement.API.Validators
{
    public partial class VoucherCreateDtoValidator : AbstractValidator<VoucherCreateDto>
    {
        public VoucherCreateDtoValidator()
        {

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Voucher code is required.")
                .Length(3, 50).WithMessage("Code must be between 3 and 50 characters.")
                .Matches(@"^[A-Z0-9]+$").WithMessage("Code must only contain uppercase letters and numbers.");

            RuleFor(x => x.DiscountPercent)
                .NotEmpty().WithMessage("Discount percentage is required.")
                .InclusiveBetween(1, 100).WithMessage("Discount percent must be between 1 and 100.");

            RuleFor(x => x.DiscountAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum spend cannot be negative.");

            RuleFor(x => x.ExpiryDate)
                .NotEmpty().WithMessage("Expiry date is required.")
                .Must(date => date.Value.Date >= DateTime.UtcNow.Date).WithMessage("Expiry date must be in the future.");

            RuleFor(x => x.UsageLimit)
                .GreaterThan(0).WithMessage("Usage limit must be greater than 0.");
        }
    }
}