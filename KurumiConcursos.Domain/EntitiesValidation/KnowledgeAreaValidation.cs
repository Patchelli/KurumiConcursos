using FluentValidation;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.ValidationHandler;

namespace KurumiConcursos.Domain.EntitiesValidation;

public sealed class KnowledgeAreaValidation : Validate<KnowledgeArea>
{
    public KnowledgeAreaValidation()
    {
        SetRules();
    }

    private void SetRules()
    {
        RuleFor(entity => entity)
            .Must(entity => entity.JourneyId > 0 || entity.Journey is not null)
            .WithMessage("Concurso obrigatório.");

        RuleFor(entity => entity.Title)
            .NotEmpty().WithMessage("Nome da disciplina obrigatório.")
            .MaximumLength(180).WithMessage("Nome da disciplina deve ter no máximo 180 caracteres.");

        RuleFor(entity => entity.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Ordem da disciplina inválida.");
    }
}
