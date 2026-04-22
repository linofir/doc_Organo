using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;

public class InternacaoConfiguration : IEntityTypeConfiguration<Internacao>
{
    public void Configure(EntityTypeBuilder<Internacao> builder)
    {
        builder.ToTable("Internacao");

        builder.HasKey(x => x.ID);

        builder.Property(x => x.Data)
            .HasColumnType("date");

        builder.Property(x => x.IndicacaoClinica)
            .HasColumnType("varchar(max)");

        builder.Property(x => x.Observacao)
            .HasColumnType("varchar(max)");

        builder.Property(x => x.CIDCodigo)
            .HasColumnType("varchar(10)");

        builder.Property(x => x.TempoDoenca)
            .HasColumnType("varchar(40)");

        builder.Property(x => x.Tipo)
            .HasColumnType("varchar(20)");

        builder.Property(x => x.Regime)
            .HasColumnType("varchar(20)");

        builder.Property(x => x.Carater)
            .HasColumnType("varchar(20)");

        builder.Property(x => x.Local)
            .HasColumnType("varchar(40)");

        builder.Property(x => x.Guia)
            .HasColumnType("varchar(40)");

        // RELACIONAMENTOS
        builder.HasOne(x => x.Prontuario)
            .WithOne(p => p.Internacao)
            .HasForeignKey<Internacao>(x => x.ProntuarioId);

        builder.HasOne(x => x.CID)
            .WithMany()
            .HasForeignKey(x => x.CIDCodigo)
            .HasPrincipalKey(c => c.Codigo);

        // INDEXES
        builder.HasIndex(x => x.ProntuarioId);
        builder.HasIndex(x => x.CIDCodigo);
        builder.HasIndex(x => x.Data);
    }
}