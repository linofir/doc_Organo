using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Infrastructure.SqlDb.Audit;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLog");

        builder.HasKey(x => x.ID);

        builder.Property(x => x.Tabela)
            .HasColumnType("varchar(100)");

        builder.Property(x => x.ValorAnterior)
            .HasColumnType("varchar(max)");

        builder.Property(x => x.ValorNovo)
            .HasColumnType("varchar(max)");

        builder.Property(x => x.Usuario)
            .HasColumnType("varchar(100)");

        builder.Property(x => x.DataOperacao)
            .HasColumnType("datetime2");

        builder.Property(x => x.Operacao)
            .HasConversion<string>()
            .HasColumnType("varchar(30)");

        builder.HasIndex(x => new { x.Tabela, x.RegistroId });
        builder.HasIndex(x => x.DataOperacao);
    }
}