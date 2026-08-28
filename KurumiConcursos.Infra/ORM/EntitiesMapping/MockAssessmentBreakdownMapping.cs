using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class MockAssessmentBreakdownMapping : MappingBase, IEntityTypeConfiguration<MockAssessmentBreakdown>
{
    public void Configure(EntityTypeBuilder<MockAssessmentBreakdown> b)
    {
        b.ToTable("mock_assessment_breakdown", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.MockAssessmentId).HasColumnName("mock_assessment_id");
        b.Property(x => x.KnowledgeAreaId).HasColumnName("knowledge_area_id");
        b.Property(x => x.TotalQuestions).HasColumnName("total_questions");
        b.Property(x => x.CorrectAnswers).HasColumnName("correct_answers");
        b.HasOne(x => x.MockAssessment).WithMany(x => x.Breakdown).HasForeignKey(x => x.MockAssessmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}