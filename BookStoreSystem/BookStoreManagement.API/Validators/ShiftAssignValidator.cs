using FluentValidation;
using BookStoreManagement.API.Models.Shift;

namespace BookStoreManagement.API.Validators
{
    public class ShiftAssignValidator : AbstractValidator<ShiftAssignDto>
    {

        public ShiftAssignValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee must be selected.");

            RuleFor(x => x.ShiftId)
               .NotEmpty().WithMessage("Shift must be selected.");

            RuleFor(x => x.WorkDate)
               .NotEmpty().WithMessage("Work date is required.")
               .Must(BeAValidDate).WithMessage("Work date cannot be in the past.");
        }

        private bool BeAValidDate(DateTime date)
        {
            return date.Date >= DateTime.Today;
        }

    }
}
