using FluentValidation;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.ValidationHandler;

namespace KurumiConcursos.Domain.EntitiesValidation;

public sealed class CalendarEventValidation : Validate<CalendarEvent>
{
    public CalendarEventValidation()
    {
        RuleFor(entity => entity.UserId)
            .NotEmpty()
            .WithMessage("Usuário obrigatório.");

        RuleFor(entity => entity.Date)
            .NotEqual(default(DateOnly))
            .WithMessage("Data do evento obrigatória.");

        RuleFor(entity => entity.Title)
            .NotEmpty()
            .WithMessage("Título do evento obrigatório.")
            .MaximumLength(255)
            .WithMessage("Título do evento deve ter no máximo 255 caracteres.");

        RuleFor(entity => entity.Type)
            .IsInEnum()
            .WithMessage("Tipo do evento inválido.");

        RuleFor(entity => entity.Note)
            .MaximumLength(1000)
            .WithMessage("Observação deve ter no máximo 1000 caracteres.");
    }
}