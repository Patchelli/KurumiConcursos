namespace KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Request;

public sealed record RegisterRequest(string Name, string Email, string Password);