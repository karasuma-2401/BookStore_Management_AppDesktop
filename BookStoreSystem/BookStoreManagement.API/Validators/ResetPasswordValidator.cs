using FluentValidation;
using BookStoreManagement.API.Models.Auth;
namespace BookStoreManagement.API.Validators
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequestDto>
    {
        public ResetPasswordValidator() {
            RuleFor(x => x.Token).NotEmpty().WithMessage("Verification code is required");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Please confirm your password")
                .Equal(x => x.NewPassword).WithMessage("Confirmation password does not match the new password");
        }
    }
}
