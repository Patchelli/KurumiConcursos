using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class MockAssessmentMapping : MappingBase, IEntityTypeConfiguration<MockAssessment>
{
    public void Configure(EntityTypeBuilder<MockAssessment> b)
    {
        b.ToTable("mock_assessment", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.AccountId).HasColumnName("account_id");
        b.Property(x => x.JourneyId).HasColumnName("journey_id");
        b.Property(x => x.Title).HasColumnName("title");
        b.Property(x => x.AssessmentDate).HasColumnName("assessment_date");
        b.Property(x => x.TotalQuestions).HasColumnName("total_questions");
        b.Property(x => x.CorrectAnswers).HasColumnName("correct_answers");
        b.Property(x => x.Score).HasColumnName("score").HasPrecision(8, 2);
    }
}