using FluentValidation;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.ValidationHandler;

namespace KurumiConcursos.Domain.EntitiesValidation;

public sealed class MemoryCardValidation : Validate<MemoryCard>
{
    public MemoryCardValidation()
    {
        RuleFor(item => item.FlashCollectionId).GreaterThan(0).WithMessage("Colecao obrigatoria.");
        RuleFor(item => item.Front).NotEmpty().WithMessage("Frente do flashcard obrigatoria.");
        RuleFor(item => item.Back).NotEmpty().WithMessage("Verso do flashcard obrigatorio.");
        RuleFor(item => item.Model).NotEmpty().MaximumLength(40).WithMessage("Modelo do flashcard invalido.");
        RuleFor(item => item.Type).NotEmpty().MaximumLength(40).WithMessage("Tipo do flashcard invalido.");
    }
}