using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.CalendarServices;

public sealed class CalendarEventService(ICalendarEventRepository repository, ICalendarEventMapper mapper)
    : ICalendarEventCommandService, ICalendarEventQueryService
{
    public async Task<CalendarEventResponse?> RegisterAsync(CalendarEventRegisterRequest request,
        UserCredential credential)
    {
        if (string.IsNullOrWhiteSpace(request.Title)) return null;
        var entity = mapper.DtoRegisterToDomain(credential.UserId, request);
        if (!await repository.SaveAsync(entity)) return null;
        return mapper.DomainToDtoResponse(entity);
    }

    public async Task<bool> UpdateAsync(CalendarEventUpdateRequest request, UserCredential credential)
    {
        var entity = await repository.FindByIdAsync(request.Id, credential.UserId, CancellationToken.None, true);
        if (entity is null) return false;
        mapper.DtoUpdateToDomain(entity, request);
        return await repository.UpdateAsync(entity);
    }

    public async Task<bool> DeleteAsync(long id, UserCredential credential)
    {
        var entity = await repository.FindByIdAsync(id, credential.UserId, CancellationToken.None, true);
        return entity is not null && await repository.DeleteAsync(entity);
    }

    public async Task<IList<CalendarEventResponse>> FindAllAsync(UserCredential credential) =>
        mapper.DomainToDtoResponseList(await repository.FindAllAsync(credential.UserId, CancellationToken.None));
}