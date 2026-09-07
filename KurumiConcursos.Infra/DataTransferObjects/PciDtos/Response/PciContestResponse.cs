namespace KurumiConcursos.Infra.DataTransferObjects.PciDtos.Response;

public sealed record PciContestResponse
{
    public required int Id { get; init; }
    public string? Titulo { get; init; }
    public string? CargosResumo { get; init; }
    public IReadOnlyList<string>? Cargos { get; init; }
    public string? Formacao { get; init; }
    public string? Regiao { get; init; }
    public string? Uf { get; init; }
    public string? VagasSalario { get; init; }
    public PciRegistrationResponse? Datas { get; init; }
    public PciNewsResponse? Noticia { get; init; }
}