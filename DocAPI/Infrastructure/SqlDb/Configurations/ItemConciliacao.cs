using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;
public class ItemConciliacaoConfig : IEntityTypeConfiguration<ItemConciliacao>
{
    public void Configure(EntityTypeBuilder<ItemConciliacao> builder)
    {
        builder.ToTable("ItemConciliacao");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ValorEsperado)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.ValorPago)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Diferenca)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Observacao)
            .HasMaxLength(500);

        builder.Property(x => x.CriadoEm)
            .IsRequired();

        builder.Property(x => x.AtualizadoEm);

        builder.Property(x => x.AtualizadoPor)
            .HasMaxLength(100);

        // Índices
        builder.HasIndex(x => x.ConciliacaoId);
        builder.HasIndex(x => x.ItemFinanceiroId);
        builder.HasIndex(x => x.AtendimentoId);
        builder.HasIndex(x => x.ProcedimentoInternacaoId);
        builder.HasIndex(x => x.Status);

        // 🔥 Compostos (core da conciliação)
        builder.HasIndex(x => new { x.ItemFinanceiroId, x.Status });
        builder.HasIndex(x => new { x.ItemFinanceiroId, x.ConciliacaoId }).IsUnique();
        builder.HasIndex(x => new { x.AtendimentoId, x.Status });
        builder.HasIndex(x => new { x.ProcedimentoInternacaoId, x.Status });
        builder.HasIndex(x => new { x.ConciliacaoId, x.Status });
        builder.HasIndex(x => new { x.Status, x.Diferenca }).IsDescending();
       

        builder.HasIndex(x => new { x.Status, x.Diferenca });
    }
}