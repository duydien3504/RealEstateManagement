using FluentValidation;
using RealEstateSystem.Application.DTOs.Request;

namespace RealEstateSystem.Application.Validators
{
    public class ChangePasswordValidation : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordValidation()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Mật khẩu cũ không được để trống.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Mật khẩu mới không được để trống.")
                .Length(6, 50).WithMessage("Mật khẩu mới phải từ 6 đến 50 ký tự.")
                .Matches(@"[A-Z]").WithMessage("Mật khẩu mới phải chứa ít nhất một chữ cái viết hoa.")
                .Matches(@"[a-z]").WithMessage("Mật khẩu mới phải chứa ít nhất một chữ cái viết thường.")
                .Matches(@"[0-9]").WithMessage("Mật khẩu mới phải chứa ít nhất một chữ số.")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("Mật khẩu mới phải chứa ít nhất một ký tự đặc biệt.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Xác nhận mật khẩu mới không được để trống.")
                .Equal(x => x.NewPassword).WithMessage("Mật khẩu xác nhận không trùng khớp.");
        }
    }
}
