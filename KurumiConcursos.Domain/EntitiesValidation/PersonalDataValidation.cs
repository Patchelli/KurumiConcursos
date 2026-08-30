using FluentValidation;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.ValidationHandler;

namespace KurumiConcursos.Domain.EntitiesValidation;

public sealed class PersonalDataValidation : Validate<PersonalData>
{
    public PersonalDataValidation()
    {
        RuleFor(entity => entity.UserId).NotEmpty().WithMessage("Usuário obrigatório.");
        RuleFor(entity => entity.FullName).MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres.");
        RuleFor(entity => entity.Document).MaximumLength(50).WithMessage("Documento deve ter no máximo 50 caracteres.");
        RuleFor(entity => entity.Phone).MaximumLength(50).WithMessage("Telefone deve ter no máximo 50 caracteres.");
    }
}