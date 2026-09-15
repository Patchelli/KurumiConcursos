using FluentValidation;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.ValidationHandler;

namespace KurumiConcursos.Domain.EntitiesValidation;

public sealed class TopicMaterialValidation : Validate<TopicMaterial>
{
    public TopicMaterialValidation()
    {
        RuleFor(item => item).Must(item => item.SyllabusNodeId.HasValue ^ item.KnowledgeAreaId.HasValue)
            .WithMessage("O material deve pertencer a uma materia ou a um topico.");
        RuleFor(item => item.Name).NotEmpty().MaximumLength(260);
        RuleFor(item => item.NextcloudPath).NotEmpty().MaximumLength(2048)
            .Must(path => path.StartsWith('/') && !path.Contains("..", StringComparison.Ordinal) && !path.Contains('\\'));
        RuleFor(item => item.MimeType).Equal("application/pdf");
    }
}
