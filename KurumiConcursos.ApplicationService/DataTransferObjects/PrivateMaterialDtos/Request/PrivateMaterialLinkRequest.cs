using System.ComponentModel.DataAnnotations;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.PrivateMaterialDtos.Request;

public sealed record PrivateMaterialLinkRequest
{
    [Required, StringLength(2048)] public string NextcloudPath { get; init; } = string.Empty;
}
