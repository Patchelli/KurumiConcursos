using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class DomainLoggerMapping : MappingBase, IEntityTypeConfiguration<DomainLogger>
{
    public void Configure(EntityTypeBuilder<DomainLogger> builder)
    {
        builder.ToTable(nameof(DomainLogger), Schema);
        builder.HasKey(domainLogger => domainLogger.Id);

        builder.Property(domainLogger => domainLogger.Id)
            .HasColumnType("bigint")
            .HasColumnName("id")
            .HasColumnOrder(1)
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn()
            .IsRequired();

        builder.Property(domainLogger => domainLogger.Action)
            .HasColumnType("smallint")
            .HasColumnName("action")
            .HasColumnOrder(2)
            .IsRequired();

        builder.Property(domainLogger => domainLogger.Description)
            .HasColumnType("text")
            .HasColumnName("description")
            .HasColumnOrder(3)
            .IsRequired();

        builder.Property(domainLogger => domainLogger.ActionDate)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("action_date")
            .HasColumnOrder(4)
            .IsRequired();

        builder.Property(domainLogger => domainLogger.UserId)
            .HasColumnType("uuid")
            .HasColumnName("user_id")
            .HasColumnOrder(5)
            .IsRequired();

        builder.Property(domainLogger => domainLogger.EntityId)
            .HasColumnType("varchar(80)")
            .HasColumnName("entity_id")
            .HasColumnOrder(6);
    }
}
