using Application.DTO.Auth;
using FluentValidation;

namespace Infrastructure.Validators;

public class RegisterPatientValidator : AbstractValidator<RegisterPatientRequest>
{
    public RegisterPatientValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Email inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.");

        RuleFor(x => x.Name.FirstName)
            .NotEmpty().WithMessage("Nome é obrigatório.");

        RuleFor(x => x.BirthDate)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("A data de nascimento deve ser válida.");
    }
}
