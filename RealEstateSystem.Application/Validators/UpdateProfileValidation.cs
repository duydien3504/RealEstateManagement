using FluentValidation;
using RealEstateSystem.Application.DTOs.Request;

namespace RealEstateSystem.Application.Validators
{
    public class UpdateProfileValidation : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileValidation()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ và tên không được để trống.")
                .Length(2, 100).WithMessage("Họ và tên phải từ 2 đến 100 ký tự.");
        }
    }
}
