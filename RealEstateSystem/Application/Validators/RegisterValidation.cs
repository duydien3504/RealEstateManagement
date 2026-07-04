using FluentValidation;
using RealEstateSystem.Application.DTOs.Request;

namespace RealEstateSystem.Application.Validators
{
    public class RegisterValidation : AbstractValidator<RegisterRequest>
    {
        public RegisterValidation() 
        {
            //Dinh dang email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không hợp lệ");

            //Dinh dang fullnam
            RuleFor(x => x.Fullname)
                .NotEmpty().WithMessage("Ho ten không được để trống")
                .MaximumLength(100).WithMessage("Họ và tên không được vượt quá 100 ký tự");

            //Dinh dang password
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
                .Matches(@"[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ hoa")
                .Matches(@"[0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 số");

            //Dinh dang confirm password
            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Mật khẩu xác nhận không khớp với mật khẩu");
        }
    }
}
