using FluentValidation;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.ValidationHandler;

namespace KurumiConcursos.Domain.EntitiesValidation;

public sealed class ExamJourneyValidation : Validate<ExamJourney>
{
    public ExamJourneyValidation()
    {
        SetRules();
    }

    private void SetRules()
    {
        RuleFor(entity => entity.AccountId)
            .NotEmpty().WithMessage("Usuário obrigatório.");

        RuleFor(entity => entity.Title)
            .NotEmpty().WithMessage("Nome do concurso obrigatório.")
            .MaximumLength(180).WithMessage("Nome do concurso deve ter no máximo 180 caracteres.");

        RuleFor(entity => entity.Salary)
            .GreaterThanOrEqualTo(0).WithMessage("Remuneração não pode ser negativa.")
            .When(entity => entity.Salary.HasValue);

        RuleFor(entity => entity.Openings)
            .GreaterThanOrEqualTo(0).WithMessage("Número de vagas não pode ser negativo.")
            .When(entity => entity.Openings.HasValue);
    }
}
