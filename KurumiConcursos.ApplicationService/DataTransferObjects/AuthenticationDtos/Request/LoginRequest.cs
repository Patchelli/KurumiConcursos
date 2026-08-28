namespace KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Request;

public sealed record LoginRequest(string Email, string Password);