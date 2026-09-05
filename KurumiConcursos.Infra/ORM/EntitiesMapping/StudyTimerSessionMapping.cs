using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class StudyTimerSessionMapping : MappingBase, IEntityTypeConfiguration<StudyTimerSession>
{
    public void Configure(EntityTypeBuilder<StudyTimerSession> b)
    {
        b.ToTable("study_timer_session", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.UserId).HasColumnName("user_id");
        b.Property(x => x.JourneyId).HasColumnName("journey_id");
        b.Property(x => x.KnowledgeAreaId).HasColumnName("knowledge_area_id");
        b.Property(x => x.SyllabusNodeId).HasColumnName("syllabus_node_id");
        b.Property(x => x.Mode).HasColumnName("mode");
        b.Property(x => x.Phase).HasColumnName("phase");
        b.Property(x => x.IsRunning).HasColumnName("is_running");
        b.Property(x => x.AccumulatedFocusSeconds).HasColumnName("accumulated_focus_seconds");
        b.Property(x => x.CurrentPhaseSeconds).HasColumnName("current_phase_seconds");
        b.Property(x => x.RunningSince).HasColumnName("running_since");
        b.Property(x => x.FocusMinutes).HasColumnName("focus_minutes");
        b.Property(x => x.ShortBreakMinutes).HasColumnName("short_break_minutes");
        b.Property(x => x.LongBreakMinutes).HasColumnName("long_break_minutes");
        b.Property(x => x.Cycles).HasColumnName("cycles");
        b.Property(x => x.CurrentCycle).HasColumnName("current_cycle");
        b.HasIndex(x => x.UserId).IsUnique();
    }
}
