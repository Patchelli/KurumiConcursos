using FluentValidation;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.ValidationHandler;

namespace KurumiConcursos.Domain.EntitiesValidation;

public sealed class StudyRoutineValidation : Validate<StudyRoutine>
{
    public StudyRoutineValidation()
    {
        RuleFor(entity => entity.UserId)
            .NotEmpty()
            .WithMessage("Usuario obrigatorio.");

        RuleFor(entity => entity.JourneyId)
            .GreaterThan(0)
            .WithMessage("Jornada obrigatoria.");

        RuleFor(entity => entity.Title)
            .NotEmpty()
            .WithMessage("Titulo do plano obrigatorio.")
            .MaximumLength(180)
            .WithMessage("Titulo do plano deve ter no maximo 180 caracteres.");

        RuleFor(entity => entity.Kind)
            .IsInEnum()
            .WithMessage("Tipo do plano invalido.");

        RuleFor(entity => entity.ConfigurationJson)
            .NotEmpty()
            .WithMessage("Configuracao do plano obrigatoria.");
    }
}