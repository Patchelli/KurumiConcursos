using System.ComponentModel.DataAnnotations;
using KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Request;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.ApplicationService.Traces;
using KurumiConcursos.Domain.Extensions;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.RadarServices;

public sealed class RadarCommandService(IUserRepository users, IRadarMapper mapper, INotificationHandler notification)
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

        var user = await users.FindByPredicateAsync(item => item.Id == credential.UserId);
        if (user is null)
        {
            notification.CreateNotification(RadarTrace.Update, "Usuário não encontrado.");
            return false;
        }

        mapper.DtoUpdateToDomain(user, request);
        var result = await users.UpdateAsync(user);
        if (!result.Succeeded)
            notification.CreateNotifications(result.SetNotificationByIdentityResult(RadarTrace.Update));
        return result.Succeeded;
    }
}