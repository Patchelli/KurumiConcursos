using System.Linq.Expressions;
using KurumiConcursos.ApplicationService.DataTransferObjects.PersonalDataDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.PersonalDataDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class PersonalDataMapper : IPersonalDataMapper
{
    public PersonalData DtoRegisterBasicToDomain(PersonalDataRegisterRequest dto) =>
        new() { FullName = dto.FullName, Document = dto.Document, Phone = dto.Phone };

    public PersonalData DtoUpdateBasicToDomain(PersonalData entity, PersonalDataUpdateRequest dto)
    {
        entity.FullName = dto.FullName ?? entity.FullName;
        entity.Document = dto.Document ?? entity.Document;
        entity.Phone = dto.Phone ?? entity.Phone;
        return entity;
    }

    public Expression<Func<PersonalData, PersonalDataResponse>> ResponseProjection() =>
        entity => new PersonalDataResponse
        {
            FullName = entity.FullName,
            Document = entity.Document,
            Phone = entity.Phone
        };

    public PersonalDataResponse DomainToDtoResponse(PersonalData entity) =>
        new() { FullName = entity.FullName, Document = entity.Document, Phone = entity.Phone };
}