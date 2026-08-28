using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class PracticeEntryMapping : MappingBase, IEntityTypeConfiguration<PracticeEntry>
{
    public void Configure(EntityTypeBuilder<PracticeEntry> b)
    {
        b.ToTable("practice_entry", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.AccountId).HasColumnName("account_id");
        b.Property(x => x.JourneyId).HasColumnName("journey_id");
        b.Property(x => x.KnowledgeAreaId).HasColumnName("knowledge_area_id");
        b.Property(x => x.SyllabusNodeId).HasColumnName("syllabus_node_id");
        b.Property(x => x.PracticeDate).HasColumnName("practice_date");
        b.Property(x => x.QuestionsAnswered).HasColumnName("questions_answered");
        b.Property(x => x.CorrectAnswers).HasColumnName("correct_answers");
    }
}