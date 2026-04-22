using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;
public class ItemFinanceiroConfig : IEntityTypeConfiguration<ItemFinanceiro>
{
    public void Configure(EntityTypeBuilder<ItemFinanceiro> builder)
    {
        builder.ToTable("ItemFinanceiro");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Lote).HasMaxLength(20);
        builder.Property(x => x.Guia).HasMaxLength(20);
        builder.Property(x => x.Protocolo).HasMaxLength(20);

        builder.Property(x => x.NomeUsuario)
            .HasMaxLength(200);

        builder.Property(x => x.TipoUsuario)
            .HasMaxLength(10);

        builder.Property(x => x.Info)
            .HasMaxLength(5);

        builder.Property(x => x.Data)
            .IsRequired();

        builder.Property(x => x.ServicoCodigo)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ServicoDescricao)
            .HasMaxLength(300);

        builder.Property(x => x.ValorTabela)
            .HasPrecision(10, 2);

        builder.Property(x => x.PercentualVia)
            .HasPrecision(5, 2);

        builder.Property(x => x.HonorarioFator)
            .HasPrecision(5, 2);

        builder.Property(x => x.ValorPago)
            .HasPrecision(10, 2);

        builder.Property(x => x.GrauParticipacao)
            .HasMaxLength(10);

        builder.Property(x => x.CriadoEm)
            .IsRequired();

        // 🔥 Índices essenciais
        builder.HasIndex(x => x.GuiaFinanceiraId);
        builder.HasIndex(x => x.Data);
        builder.HasIndex(x => x.ServicoCodigo);
        builder.HasIndex(x => x.Guia);
        builder.HasIndex(x => x.Protocolo);
        builder.HasIndex(x => x.Lote);

        // 🔥 índice composto (muito importante)
        builder.HasIndex(x => new { x.Guia, x.ServicoCodigo });
        builder.HasIndex(x => new { x.ServicoCodigo, x.Data });
    }
}
