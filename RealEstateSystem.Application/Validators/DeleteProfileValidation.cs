using FluentValidation;
using RealEstateSystem.Application.DTOs.Request;

namespace RealEstateSystem.Application.Validators
{
    public class DeleteProfileValidation : AbstractValidator<DeleteProfileRequest>
    {
        public DeleteProfileValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống.")
                .EmailAddress().WithMessage("Email không đúng định dạng.");
        }
    }
}
