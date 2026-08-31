using System.Text.Json;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class StudyRoutineMapper : IStudyRoutineMapper
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public StudyRoutine DtoRegisterToDomain(Guid userId, StudyRoutineRegisterRequest dto) => new()
    {
        UserId = userId, JourneyId = dto.JourneyId, Title = dto.Title.Trim(), Kind = dto.Kind, Active = true,
        ConfigurationJson = JsonSerializer.Serialize(dto.Configuration, Options)
    };

    public StudyRoutine DtoUpdateToDomain(StudyRoutine entity, StudyRoutineUpdateRequest dto)
    {
        entity.Title = dto.Title.Trim();
        entity.Kind = dto.Kind;
        entity.ConfigurationJson = JsonSerializer.Serialize(dto.Configuration, Options);
        entity.LastUpdateDate = DateTimeOffset.UtcNow;
        return entity;
    }

    public StudyRoutineResponse DomainToDtoResponse(StudyRoutine entity) => new(entity.Id, entity.JourneyId,
        entity.Title, entity.Kind, entity.Active,
        JsonSerializer.Deserialize<StudyRoutineConfigurationRequest>(entity.ConfigurationJson, Options) ?? new([],
            new Dictionary<long, string>(), 2, 7, 50, 25, 25, new Dictionary<string, decimal>(),
            new Dictionary<long, decimal>(),
            new Dictionary<long, decimal>()));
}