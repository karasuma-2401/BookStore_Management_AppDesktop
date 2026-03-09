using FluentValidation; 
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            // Thiet ke cho Username
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username khong duoc de trong")
                .MaximumLength(100).WithMessage("Username khong duoc qua 100 ky tu");

            // Thiet ke cho Password (luc nay van la chuoi thô truoc khi bam)
            RuleFor(x => x.PasswordHash)
                .NotEmpty().WithMessage("Mat khau khong duoc de trong")
                .MinimumLength(6).WithMessage("Mat khau phai co it nhat 6 ky tu");

            // Thiet ke cho FullName (vi trong Model ban de string? nen ta chi check do dai)
            RuleFor(x => x.FullName)
                .MaximumLength(200).WithMessage("Ho ten khong duoc qua 200 ky tu");

            // Thiet ke cho RoleId (dam bao chi co 'admin' hoac 'staff')
            RuleFor(x => x.RoleId)
                .Must(role => role == "admin" || role == "staff")
                .WithMessage("Role chi duoc phep la admin hoac staff");
        }
    }
}
