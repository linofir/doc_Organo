using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;
public class GuiaFinanceiraConfig : IEntityTypeConfiguration<GuiaFinanceira>
{
    public void Configure(EntityTypeBuilder<GuiaFinanceira> builder)
    {
        builder.ToTable("GuiaFinanceira");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TipoGuia)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TipoGuiaDescricao)
            .HasMaxLength(200);

        builder.Property(x => x.CriadoEm)
            .IsRequired();

        builder.HasIndex(x => x.DemonstrativoFinanceiroId);
        builder.HasIndex(x => x.TipoGuia);

        builder.HasMany(x => x.Itens)
            .WithOne(x => x.GuiaFinanceira)
            .HasForeignKey(x => x.GuiaFinanceiraId);
    }
}