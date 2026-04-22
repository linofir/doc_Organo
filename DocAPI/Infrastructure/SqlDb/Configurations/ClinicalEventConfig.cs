using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;

public class ClinicalEventConfiguration : IEntityTypeConfiguration<ClinicalEvent>
{
    public void Configure(EntityTypeBuilder<ClinicalEvent> builder)
    {
        builder.ToTable("ClinicalEvent");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TipoEvento)     
            .HasConversion<string>()
            .HasColumnType("varchar(30)");

        builder.Property(x => x.Descricao)
            .HasMaxLength(200);

        builder.Property(x => x.Usuario)
            .HasMaxLength(50);

        builder.Property(x => x.DataEvento)
            .HasColumnType("datetime2");

        builder.HasOne(x => x.Atendimento)
            .WithMany(a => a.Eventos)
            .HasForeignKey(x => x.AtendimentoId);

        builder.HasIndex(x => x.AtendimentoId);

        builder.HasIndex(x => x.DataEvento);

        builder.HasIndex(x => x.TipoEvento);
    }
}