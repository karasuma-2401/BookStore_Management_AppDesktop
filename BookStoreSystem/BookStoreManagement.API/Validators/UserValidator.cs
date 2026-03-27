using FluentValidation; 
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MaximumLength(100).WithMessage("Username cannot exceed 100 characters");

            RuleFor(x => x.PasswordHash)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long");

            RuleFor(x => x.FullName)
                .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters");

            RuleFor(x => x.RoleId)
                .Must(role => role == "admin" || role == "staff")
                .WithMessage("Role must be either 'admin' or 'staff'");
        }
    }
}
