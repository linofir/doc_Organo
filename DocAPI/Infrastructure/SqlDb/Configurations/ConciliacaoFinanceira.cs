using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;

public class ConciliacaoFinanceiraConfig : IEntityTypeConfiguration<ConciliacaoFinanceira>
{
    public void Configure(EntityTypeBuilder<ConciliacaoFinanceira> builder)
    {
        builder.ToTable("ConciliacaoFinanceira");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Periodo)
            .IsRequired();

        builder.Property(x => x.CriadoEm)
            .IsRequired();

        builder.Property(x => x.AtualizadoEm);

        builder.Property(x => x.AtualizadoPor)
            .HasMaxLength(100);

        builder.HasIndex(x => x.Periodo);
        builder.HasIndex(x => x.CriadoEm);

        builder.HasMany(x => x.Itens)
            .WithOne(x => x.Conciliacao)
            .HasForeignKey(x => x.ConciliacaoId);
    }
}