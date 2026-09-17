using FluentValidation;
using RealEstateSystem.Application.DTOs.Request;

namespace RealEstateSystem.Application.Validators
{
    public class UpRoleValidation : AbstractValidator<UpRoleRequest>
    {
        public UpRoleValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống.")
                .EmailAddress().WithMessage("Định dạng Email không hợp lệ.")
                .MaximumLength(150).WithMessage("Email không được vượt quá 150 ký tự.");

            RuleFor(x => x.IdCardNumber)
                .NotEmpty().WithMessage("Số CCCD không được để trống.")
                .MaximumLength(20).WithMessage("Số CCCD không được vượt quá 20 ký tự.");
        }
    }
}
