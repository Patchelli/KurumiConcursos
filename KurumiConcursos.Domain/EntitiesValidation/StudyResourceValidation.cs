using FluentValidation;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.ValidationHandler;

namespace KurumiConcursos.Domain.EntitiesValidation;

public sealed class StudyResourceValidation : Validate<StudyResource>
{
    public StudyResourceValidation()
    {
        RuleFor(entity => entity.UserId)
            .NotEmpty()
            .WithMessage("Usuario obrigatorio.");

        RuleFor(entity => entity.JourneyId)
            .GreaterThan(0)
            .WithMessage("Jornada obrigatoria.");

        RuleFor(entity => entity.Title)
            .NotEmpty()
            .WithMessage("Titulo do material obrigatorio.")
            .MaximumLength(180)
            .WithMessage("Titulo do material deve ter no maximo 180 caracteres.");

        RuleFor(entity => entity.Url)
            .NotEmpty()
            .WithMessage("URL do material obrigatoria.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("URL do material invalida.");

        RuleFor(entity => entity.Kind)
            .IsInEnum()
            .WithMessage("Tipo do material invalido.");
    }
}