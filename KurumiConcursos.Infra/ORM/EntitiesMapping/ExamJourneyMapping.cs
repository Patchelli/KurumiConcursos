using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class ExamJourneyMapping : MappingBase, IEntityTypeConfiguration<ExamJourney>
{
    public void Configure(EntityTypeBuilder<ExamJourney> b)
    {
        b.ToTable("exam_journey", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.UserId).HasColumnName("user_id");
        b.Property(x => x.Title).HasColumnName("title").HasMaxLength(180);
        b.Property(x => x.Institution).HasColumnName("institution").HasMaxLength(180);
        b.Property(x => x.ExamBoard).HasColumnName("exam_board").HasMaxLength(120);
        b.Property(x => x.Position).HasColumnName("position").HasMaxLength(180);
        b.Property(x => x.Salary).HasColumnName("salary").HasPrecision(14, 2);
        b.Property(x => x.Openings).HasColumnName("openings");
        b.Property(x => x.NoticeUrl).HasColumnName("notice_url");
        b.Property(x => x.ExamDate).HasColumnName("exam_date");
        b.Property(x => x.Stage).HasColumnName("stage");
        b.Property(x => x.IncludeInStatistics).HasColumnName("include_in_statistics");
        b.Property(x => x.CompletedSyllabusCycles).HasColumnName("completed_syllabus_cycles");
        b.Property(x => x.LogoUrl).HasColumnName("logo_url");
        b.HasOne(x => x.StudentProfile).WithMany(x => x.ExamJourneys).HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}