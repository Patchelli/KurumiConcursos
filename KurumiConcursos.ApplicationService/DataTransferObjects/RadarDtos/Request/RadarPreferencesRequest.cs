using System.ComponentModel.DataAnnotations;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Request;

public sealed record RadarPreferencesRequest
{
    [Required(AllowEmptyStrings = true), RegularExpression("^(|norte|nordeste|centro-oeste|sudeste|sul)$")]
    public string Region { get; init; } = "";

    [Required(AllowEmptyStrings = true),
     RegularExpression("^(|AC|AL|AP|AM|BA|CE|DF|ES|GO|MA|MT|MS|MG|PA|PB|PR|PE|PI|RJ|RN|RS|RO|RR|SC|SP|SE|TO)$")]
    public string State { get; init; } = "";

    [Required(AllowEmptyStrings = true), RegularExpression("^(|fundamental|medio|tecnico|superior)$")]
    public string Education { get; init; } = "";

    [Required(AllowEmptyStrings = true), StringLength(100)]
    public string Role { get; init; } = "";

    public bool IncludeNational { get; init; } = true;
}