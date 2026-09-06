namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyOverviewDtos.Response;

public sealed record JourneyOverviewDayResponse(int DayOfWeek, int StudiedMinutes, int Questions, decimal? Accuracy);