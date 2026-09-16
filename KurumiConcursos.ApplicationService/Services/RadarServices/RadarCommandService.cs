using System.ComponentModel.DataAnnotations;
using KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Request;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.ApplicationService.Traces;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.RadarServices;

public sealed class RadarCommandService(
    IStudentProfileRepository profiles,
    IRadarMapper mapper,
    INotificationHandler notification)
    : IRadarCommandService
{
    public async Task<bool> SavePreferencesAsync(RadarPreferencesRequest request, UserCredential credential)
    {
        var errors = new List<ValidationResult>();
        if (!Validator.TryValidateObject(request, new ValidationContext(request), errors, true))
        {
            notification.CreateNotification(RadarTrace.Update, "Verifique os filtros informados.");
            return false;
        }

        var profile = await profiles.FindByPredicateAsync(item => item.UserId == credential.UserId);
        if (profile is null)
        {
            notification.CreateNotification(RadarTrace.Update, "Usuário não encontrado.");
            return false;
        }

        mapper.DtoUpdateToDomain(profile, request);
        var result = await profiles.UpdateAsync(profile);
        if (!result)
            notification.CreateNotification(RadarTrace.Update, "Nao foi possivel salvar as preferencias.");
        return result;
    }
}