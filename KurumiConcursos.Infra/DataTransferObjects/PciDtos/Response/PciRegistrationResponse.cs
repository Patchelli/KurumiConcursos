namespace KurumiConcursos.Infra.DataTransferObjects.PciDtos.Response;

public sealed record PciRegistrationResponse(bool Aberto, string? Inicio, string? Fim);