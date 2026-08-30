using KurumiConcursos.ApplicationService.DataTransferObjects.PersonalDataDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.UserDtos.Request;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.ApplicationService.Traces;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.Extensions;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.UserServices;

public sealed class UserCommandService(
    IPersonalDataRepository personalDataRepository,
    IPersonalDataMapper personalDataMapper,
    IUserRepository userRepository,
    INotificationHandler notification,
    IValidate<PersonalData> validation,
    ILoggerHandler logger) : ServiceBase<PersonalData>(notification, validation, logger), IUserCommandService
{
    public async Task<bool> UpdateMyPersonalDataAsync(
        PersonalDataUpdateRequest request,
        UserCredential credential)
    {
        var personalData = await personalDataRepository.FindByPredicateAsync(item => item.UserId == credential.UserId);
        if (personalData is null)
        {
            Notification.CreateNotification(UserTrace.Update, "Dados pessoais não encontrados.");
            return false;
        }

        personalDataMapper.DtoUpdateBasicToDomain(personalData, request);
        if (!await EntityValidationAsync(personalData))
            return false;

        GenerateLogger(EUserAction.Update, UserTrace.Update, credential.UserId, personalData.Id.ToString());
        return await personalDataRepository.UpdateAsync(personalData);
    }

    public async Task<bool> ChangePasswordAsync(
        UserChangePasswordRequest request,
        UserCredential credential)
    {
        var user = await userRepository.FindByPredicateAsync(item => item.Id == credential.UserId);
        if (user is null)
        {
            Notification.CreateNotification(UserTrace.ChangePassword, "Usuário não encontrado.");
            return false;
        }

        var result = await userRepository.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            Notification.CreateNotifications(result.SetNotificationByIdentityResult(UserTrace.ChangePassword));
            return false;
        }

        GenerateLogger(EUserAction.Update, UserTrace.ChangePassword, credential.UserId, user.Id.ToString());
        return true;
    }
}