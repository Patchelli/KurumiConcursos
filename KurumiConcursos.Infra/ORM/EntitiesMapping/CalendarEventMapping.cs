using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class CalendarEventMapping : MappingBase, IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.ToTable("calendar_event", Schema);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnType("bigint").HasColumnName("id").HasColumnOrder(1).ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn().IsRequired();
        builder.Property(x => x.UserId).HasColumnType("uuid").HasColumnName("user_id").HasColumnOrder(2).IsRequired();
        builder.Property(x => x.Date).HasColumnName("event_date").HasColumnOrder(3).IsRequired();
        builder.Property(x => x.Title).HasColumnType("varchar(255)").HasColumnName("title").HasColumnOrder(4)
            .IsRequired();
        builder.Property(x => x.Type).HasColumnName("type").HasColumnOrder(5).IsRequired();
        builder.Property(x => x.Note).HasColumnType("varchar(1000)").HasColumnName("note").HasColumnOrder(6)
            .IsRequired(false);
        builder.Property(x => x.CreationDate).HasColumnType("timestamptz").HasColumnName("creation_date")
            .HasColumnOrder(7).IsRequired();
        builder.Property(x => x.LastUpdateDate).HasColumnType("timestamptz").HasColumnName("last_update_date")
            .HasColumnOrder(8).IsRequired(false);
        builder.HasIndex(x => x.UserId).HasDatabaseName("ix_calendar_event_user_id");
    }
}