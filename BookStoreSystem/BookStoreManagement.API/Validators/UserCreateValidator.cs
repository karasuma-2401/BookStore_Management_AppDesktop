using FluentValidation; 
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Models.Auth;

namespace BookStoreManagement.API.Validators
{
    public class UserCreateValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateValidator()
        {

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MaximumLength(100).WithMessage("Username cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long");

            RuleFor(x => x.FullName)
                .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.FullName));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.RoleId)
                .Must(role => role == "admin" || role == "staff")
                .WithMessage("Role must be either 'admin' or 'staff'");
        }
    }
}
