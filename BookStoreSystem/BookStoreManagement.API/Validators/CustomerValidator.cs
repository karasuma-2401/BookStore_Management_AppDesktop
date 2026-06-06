using FluentValidation;
using BookStoreManagement.API.Models.DTOs;

public class CustomerValidator : AbstractValidator<CustomerCreateDto>
{
    public CustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required")
            .Matches(@"^[0-9]{10,11}$")
            .WithMessage("Phone must be 10-11 digits");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .When(x => x.Address != null);
    }
}