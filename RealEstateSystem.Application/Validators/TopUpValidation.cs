using FluentValidation;
using RealEstateSystem.Application.DTOs.Request;

namespace RealEstateSystem.Application.Validators
{
    public class TopUpValidation : AbstractValidator<TopUpRequest>
    {
        public TopUpValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống.")
                .EmailAddress().WithMessage("Định dạng Email không hợp lệ.")
                .MaximumLength(150).WithMessage("Email không được vượt quá 150 ký tự.");

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(50000).WithMessage("Số tiền nạp tối thiểu là 50,000 VND.")
                .LessThanOrEqualTo(50000000).WithMessage("Số tiền nạp tối đa là 50,000,000 VND.");
        }
    }
}
