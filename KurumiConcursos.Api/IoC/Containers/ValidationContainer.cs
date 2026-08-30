using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.EntitiesValidation;
using KurumiConcursos.Domain.Interface;

namespace KurumiConcursos.Api.IoC.Containers;

public static class ValidationContainer
{
    public static IServiceCollection AddValidationContainer(this IServiceCollection services)
    {
        services
            .AddScoped<IValidate<ExamJourney>, ExamJourneyValidation>()
            .AddScoped<IValidate<KnowledgeArea>, KnowledgeAreaValidation>()
            .AddScoped<IValidate<SyllabusNode>, SyllabusNodeValidation>()
            .AddScoped<IValidate<PersonalData>, PersonalDataValidation>();

        return services;
    }
}
