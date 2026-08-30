using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class CalendarEventMapping : MappingBase, IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> b)
    {
        b.ToTable("calendar_event", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.UserId).HasColumnName("user_id");
        b.Property(x => x.Date).HasColumnName("event_date");
        b.Property(x => x.Title).HasColumnName("title");
        b.Property(x => x.Type).HasColumnName("type");
        b.Property(x => x.Note).HasColumnName("note");
    }
}