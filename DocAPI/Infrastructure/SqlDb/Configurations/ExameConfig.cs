using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;

public class ExameConfiguration : IEntityTypeConfiguration<Exame>
{
    public void Configure(EntityTypeBuilder<Exame> builder)
    {
        builder.ToTable("Exame");

        builder.HasKey(x => x.ID);

        builder.Property(x => x.Codigo)
            .HasColumnType("varchar(20)");

        builder.Property(x => x.Nome)
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnType("varchar(40)");

        builder.Property(x => x.DataSolicitacao)
            .HasColumnType("date");

        builder.Property(x => x.DataResultado)
            .HasColumnType("date");

        // RELACIONAMENTO
        builder.HasOne(x => x.Prontuario)
            .WithMany(p => p.Exames)
            .HasForeignKey(x => x.ProntuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // INDEXES
        builder.HasIndex(x => x.ProntuarioId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.DataSolicitacao);
    }
}