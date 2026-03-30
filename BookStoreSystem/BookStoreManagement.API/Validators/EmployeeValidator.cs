using FluentValidation;
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Validators
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {

        public EmployeeValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters");
            RuleFor(x => x.Age)
                .NotEmpty().WithMessage("Age is required")
                .GreaterThanOrEqualTo(18).WithMessage("Employee must be at least 18 years old")
                .LessThanOrEqualTo(65).WithMessage("Employee must be no older than 65 years old");
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required")
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters");
            RuleFor(x => x.Salary)
                .GreaterThanOrEqualTo(0).WithMessage("Salary must be a positive number");
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required");

        }

    }
}
