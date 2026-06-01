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
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime nowVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);
            return date.Date >= nowVn.Date;
        }

    }
}
