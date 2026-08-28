namespace KurumiConcursos.ApplicationService.Traces;

public static class UserTrace
{
    public const string Save = "Cadastro de usuário";
    public const string Update = "Atualização de usuário";
    public const string Delete = "Exclusão de usuário";
    public const string ChangeStatus = "Alteração de status do usuário";
    public const string ChangePassword = "Alteração de senha do usuário";
    public const string ActivateOrDeactivate = "Ativação ou desativação de usuário";
    public const string InvalidPassword = "Senha inválida";
    public const string UpdateLanguage = "User.UpdateLanguage";
    public const string ApproveProfile = "Aprovação de perfil do usuário";
    public const string PrepareRegistration = "Preparação de registro de usuário";
}
