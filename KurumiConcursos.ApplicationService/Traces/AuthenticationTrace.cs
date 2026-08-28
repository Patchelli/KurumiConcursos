namespace KurumiConcursos.ApplicationService.Traces;

public static class AuthenticationTrace
{
    public const string AccessToken = "Geração do token de Autenticação";
    public const string RefreshToken = "Geração do token de Atualização";
    public const string AccessOrRefreshToken = "Geração ou atualização de token";
    public const string LoginOrPassword = "Login ou senha inválido";
    public const string IdNotFound = "Identificador não encontrado";
    public const string UserInactiveOrWithoutPermission = "Inativo ou sem permissão";
    public const string SecurityExtraction = "Erro ao extrair credenciais do token";
}
