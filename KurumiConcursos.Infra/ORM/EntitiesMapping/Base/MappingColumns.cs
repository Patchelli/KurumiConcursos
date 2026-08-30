using KurumiConcursos.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping.Base;

internal static class MappingColumns
{
    public static void Base<T>(EntityTypeBuilder<T> b) where T : EntityBase
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint").ValueGeneratedOnAdd();
        b.Property(x => x.CreationDate).HasColumnName("creation_date").HasColumnType("timestamp with time zone");
        b.Property(x => x.LastUpdateDate).HasColumnName("last_update_date").HasColumnType("timestamp with time zone");
    }
}