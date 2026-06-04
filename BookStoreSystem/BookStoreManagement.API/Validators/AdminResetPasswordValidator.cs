using FluentValidation;
using BookStoreManagement.API.Models.Auth;

namespace BookStoreManagement.API.Validators
{
    public class AdminResetPasswordValidator : AbstractValidator<AdminResetPasswordDto>
    {
        public AdminResetPasswordValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
        }
    }
}