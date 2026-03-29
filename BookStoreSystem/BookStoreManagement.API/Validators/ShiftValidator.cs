using FluentValidation;
using BookStoreManagement.API.Models.Shift;

namespace BookStoreManagement.API.Validators
{
    public class UpdateShiftTimeValidator : AbstractValidator<ShiftTimeUpdateDto>
    {
        public UpdateShiftTimeValidator()
        {
            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required.");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required.")
                .Must((dto, endTime) => endTime > dto.StartTime)
                .WithMessage("End time must be later than Start time.");
        }
    }
}
