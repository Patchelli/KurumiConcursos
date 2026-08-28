namespace KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Response;

public sealed record AuthenticationResponse(string AccessToken, string Name, string Email);