using FluentValidation;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.ValidationHandler;

namespace KurumiConcursos.Domain.EntitiesValidation;

public sealed class SyllabusNodeValidation : Validate<SyllabusNode>
{
    public SyllabusNodeValidation()
    {
        SetRules();
    }

    private void SetRules()
    {
        RuleFor(entity => entity)
            .Must(entity => entity.KnowledgeAreaId > 0 || entity.KnowledgeArea is not null)
            .WithMessage("Disciplina obrigatória.");

        RuleFor(entity => entity.Title)
            .NotEmpty().WithMessage("Nome do tópico obrigatório.")
            .MaximumLength(300).WithMessage("Nome do tópico deve ter no máximo 300 caracteres.");

        RuleFor(entity => entity.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Ordem do tópico inválida.");
    }
}
