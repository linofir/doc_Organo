using DocAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class ProntuarioAcaoCDConfiguration : IEntityTypeConfiguration<ProntuarioAcaoCD>
{
    public void Configure(EntityTypeBuilder<ProntuarioAcaoCD> builder)
    {
        builder.ToTable("ProntuarioAcaoCD");

        builder.HasKey(x => x.ID);

        builder.Property(x => x.Tipo)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(x => x.Prontuario)
            .WithMany(p => p.AcoesCD)
            .HasForeignKey(x => x.ProntuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProntuarioId);
    }
}