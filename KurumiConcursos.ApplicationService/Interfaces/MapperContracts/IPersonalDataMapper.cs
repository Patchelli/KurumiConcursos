using System.Linq.Expressions;
using KurumiConcursos.ApplicationService.DataTransferObjects.PersonalDataDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.PersonalDataDtos.Response;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface IPersonalDataMapper
{
    PersonalData DtoRegisterBasicToDomain(PersonalDataRegisterRequest dto);
    PersonalData DtoUpdateBasicToDomain(PersonalData entity, PersonalDataUpdateRequest dto);
    Expression<Func<PersonalData, PersonalDataResponse>> ResponseProjection();
    PersonalDataResponse DomainToDtoResponse(PersonalData entity);
}