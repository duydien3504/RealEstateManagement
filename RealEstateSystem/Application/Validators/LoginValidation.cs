using FluentValidation;
using RealEstateSystem.Application.DTOs.Request;

namespace RealEstateSystem.Application.Validators
{
    public class LoginValidation : AbstractValidator<LoginRequest>
    {
        public LoginValidation()
        {
            //Dinh dang email
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không hợp lệ");

            //Dinh dang password
            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống");
        }
    }
}
