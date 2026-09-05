using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyTimerDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyTimerDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.ApplicationService.Traces;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.StudyTimerServices;

public sealed class StudyTimerCommandService(
    IStudyTimerSessionRepository repository,
    IFocusSessionRepository focusSessionRepository,
    IJourneyRepository journeyRepository,
    ISyllabusNodeStudyCommandService nodeStudyCommandService,
    IStudyTimerMapper mapper,
    IValidate<StudyTimerSession> validation,
    INotificationHandler notification,
    ILoggerHandler logger)
    : ServiceBase<StudyTimerSession>(notification, validation, logger), IStudyTimerCommandService
{
    public async Task<StudyTimerResponse?> SaveAsync(StudyTimerSaveRequest request, UserCredential credential)
    {
        var journey = await journeyRepository.FindByIdAsync(request.JourneyId, credential.UserId,
            CancellationToken.None, includeStructure: true);
        var area = journey?.KnowledgeAreas.FirstOrDefault(x => x.Id == request.KnowledgeAreaId);
        var nodeIsValid = !request.SyllabusNodeId.HasValue ||
                          area?.SyllabusNodes.Any(x => x.Id == request.SyllabusNodeId.Value) == true;
        if (area is null || !nodeIsValid)
        {
            Notification.CreateNotification(StudyTimerTrace.Save, "Contexto de estudo invalido.");
            return null;
        }

        var now = DateTimeOffset.UtcNow;
        var session = await repository.FindByUserAsync(credential.UserId, tracking: true);
        if (session is null)
        {
            session = mapper.DtoSaveToDomain(credential.UserId, request, now);
            if (!await EntityValidationAsync(session)) return null;
            if (!await repository.SaveAsync(session))
            {
                Notification.CreateNotification(StudyTimerTrace.Save, "Nao foi possivel iniciar o temporizador.");
                return null;
            }
            GenerateLogger(EUserAction.Save, StudyTimerTrace.Save, credential.UserId, session.Id.ToString());
        }
        else
        {
            var current = mapper.DomainToDtoResponse(session, now);
            if ((session.KnowledgeAreaId != request.KnowledgeAreaId || session.SyllabusNodeId != request.SyllabusNodeId) &&
                current.AccumulatedFocusSeconds > 0)
            {
                Notification.CreateNotification(StudyTimerTrace.Save,
                    "Finalize ou descarte a sessao atual antes de trocar o conteudo.");
                return null;
            }
            var normalized = request with
            {
                AccumulatedFocusSeconds = Math.Max(request.AccumulatedFocusSeconds, current.AccumulatedFocusSeconds),
                CurrentPhaseSeconds = request.Phase == session.Phase
                    ? Math.Max(request.CurrentPhaseSeconds, current.CurrentPhaseSeconds)
                    : request.CurrentPhaseSeconds
            };
            mapper.DtoSaveToDomain(normalized, session, now);
            if (!await EntityValidationAsync(session)) return null;
            if (!await repository.UpdateAsync(session))
            {
                Notification.CreateNotification(StudyTimerTrace.Save, "Nao foi possivel atualizar o temporizador.");
                return null;
            }
            GenerateLogger(EUserAction.Update, StudyTimerTrace.Save, credential.UserId, session.Id.ToString());
        }
        return mapper.DomainToDtoResponse(session, now);
    }

    public async Task<bool> FinishAsync(StudyTimerFinishRequest request, UserCredential credential)
    {
        var session = await repository.FindByUserAsync(credential.UserId, tracking: true);
        if (session is null) return Notification.CreateNotification(StudyTimerTrace.Finish, "Sessao ativa nao encontrada.");
        var current = mapper.DomainToDtoResponse(session, DateTimeOffset.UtcNow);
        if (current.AccumulatedFocusSeconds <= 0)
            return Notification.CreateNotification(StudyTimerTrace.Finish, "Nao ha tempo de foco para registrar.");

        if (session.SyllabusNodeId.HasValue)
        {
            var result = await nodeStudyCommandService.SaveAsync(new SyllabusNodeStudyRequest(
                session.JourneyId, session.SyllabusNodeId.Value, request.Completed,
                0, false, null, false, current.AccumulatedFocusSeconds), credential);
            if (result is null) return false;
        }
        else
        {
            var focus = new FocusSession
            {
                UserId = credential.UserId, JourneyId = session.JourneyId,
                KnowledgeAreaId = session.KnowledgeAreaId, StudyDate = CurrentDate(),
                DurationSeconds = current.AccumulatedFocusSeconds,
                Notes = request.Completed ? "Concluido pelo temporizador" : "Pendente pelo temporizador"
            };
            if (!await focusSessionRepository.SaveAsync(focus))
                return Notification.CreateNotification(StudyTimerTrace.Finish, "Nao foi possivel registrar o tempo estudado.");
        }

        if (!await repository.DeleteAsync(session))
            return Notification.CreateNotification(StudyTimerTrace.Finish, "O tempo foi registrado, mas a sessao ativa nao foi encerrada.");
        GenerateLogger(EUserAction.Save, StudyTimerTrace.Finish, credential.UserId, session.Id.ToString());
        return true;
    }

    public async Task<bool> DiscardAsync(UserCredential credential)
    {
        var session = await repository.FindByUserAsync(credential.UserId, tracking: true);
        if (session is null) return true;
        if (!await repository.DeleteAsync(session))
            return Notification.CreateNotification(StudyTimerTrace.Discard, "Nao foi possivel descartar o temporizador.");
        GenerateLogger(EUserAction.Delete, StudyTimerTrace.Discard, credential.UserId, session.Id.ToString());
        return true;
    }

    private static DateOnly CurrentDate()
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "E. South America Standard Time" : "America/Sao_Paulo");
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone));
    }
}
