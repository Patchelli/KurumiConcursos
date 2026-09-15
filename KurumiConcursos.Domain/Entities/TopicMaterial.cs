using KurumiConcursos.Domain.Entities.Base;

namespace KurumiConcursos.Domain.Entities;

public sealed class TopicMaterial : EntityBase
{
    public long? SyllabusNodeId { get; set; }
    public SyllabusNode? SyllabusNode { get; set; }
    public long? KnowledgeAreaId { get; set; }
    public KnowledgeArea? KnowledgeArea { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NextcloudPath { get; set; } = string.Empty;
    public string MimeType { get; set; } = "application/pdf";
}
