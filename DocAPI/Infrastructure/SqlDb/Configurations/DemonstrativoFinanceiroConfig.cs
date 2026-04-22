using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;

public class DemonstrativoFinanceiroConfig : IEntityTypeConfiguration<DemonstrativoFinanceiro>
{
    public void Configure(EntityTypeBuilder<DemonstrativoFinanceiro> builder)
    {
        builder.ToTable("DemonstrativoFinanceiro");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Periodo)
            .IsRequired();

        builder.Property(x => x.JsonPath)
            .HasMaxLength(500);

        builder.Property(x => x.CriadoEm)
            .IsRequired();

        // Relacionamento
        builder.HasMany(x => x.Guias)
            .WithOne(x => x.DemonstrativoFinanceiro)
            .HasForeignKey(x => x.DemonstrativoFinanceiroId);
    }
}