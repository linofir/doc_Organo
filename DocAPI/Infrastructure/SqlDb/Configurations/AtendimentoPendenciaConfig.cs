using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;

namespace DocAPI.Infrastructure.SqlDb.Configurations;

public class AtendimentoPendenciaConfiguration : IEntityTypeConfiguration<AtendimentoPendencia>
{
    public void Configure(EntityTypeBuilder<AtendimentoPendencia> builder)
    {
        builder.ToTable("AtendimentoPendencia");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Tipo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Descricao)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.CriadoEm)
            .HasColumnType("datetime2");

        builder.HasIndex(x => x.AtendimentoId);
    }
}
