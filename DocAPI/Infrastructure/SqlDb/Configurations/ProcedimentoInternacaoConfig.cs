using DocAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class ProcedimentoInternacaoConfiguration : IEntityTypeConfiguration<ProcedimentoInternacao>
{
    public void Configure(EntityTypeBuilder<ProcedimentoInternacao> builder)
    {
        builder.ToTable("ProcedimentoInternacao");

        builder.HasKey(x => x.ID);

        builder.Property(x => x.CodigoProcedimento)
            .HasColumnType("varchar(20)");

        builder.Property(x => x.Descricao)
            .HasColumnType("varchar(200)");

        builder.HasOne(x => x.Internacao)
            .WithMany(i => i.Procedimentos)
            .HasForeignKey(x => x.InternacaoId);

        builder.HasIndex(x => x.InternacaoId); // ❌ removi unique (estava errado)
    }
}