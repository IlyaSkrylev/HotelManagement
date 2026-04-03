namespace HotelManagement.API.Features.Hotels.CreateHotel
{
    public class CreateHotelValidator : AbstractValidator<CreateHotelCommand>
    {
        public CreateHotelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название гостиницы обязательно")
                .MaximumLength(255).WithMessage("Название не должно превышать 255 символов");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Неверный формат email")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Телефон не должен превышать 20 символов");
        }
    }
}
