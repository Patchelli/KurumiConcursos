using FluentValidation;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.ValidationHandler;

namespace KurumiConcursos.Domain.EntitiesValidation;

public sealed class StudyTimerSessionValidation : Validate<StudyTimerSession>
{
    public StudyTimerSessionValidation()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Usuario obrigatorio.");
        RuleFor(x => x.JourneyId).GreaterThan(0).WithMessage("Jornada obrigatoria.");
        RuleFor(x => x.KnowledgeAreaId).GreaterThan(0).WithMessage("Materia obrigatoria.");
        RuleFor(x => x.Mode).IsInEnum().WithMessage("Modo do temporizador invalido.");
        RuleFor(x => x.Phase).IsInEnum().WithMessage("Fase do temporizador invalida.");
        RuleFor(x => x.AccumulatedFocusSeconds).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrentPhaseSeconds).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FocusMinutes).InclusiveBetween(1, 120);
        RuleFor(x => x.ShortBreakMinutes).InclusiveBetween(1, 60);
        RuleFor(x => x.LongBreakMinutes).InclusiveBetween(1, 60);
        RuleFor(x => x.Cycles).InclusiveBetween(1, 12);
        RuleFor(x => x.CurrentCycle).GreaterThan(0);
    }
}
