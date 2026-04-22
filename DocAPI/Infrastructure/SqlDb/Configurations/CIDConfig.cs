using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;

public class CIDConfiguration : IEntityTypeConfiguration<CID>
{
    public void Configure(EntityTypeBuilder<CID> builder)
    {
        builder.ToTable("CID");

        builder.HasKey(x => x.Codigo);

        builder.Property(x => x.Codigo)
            .HasColumnType("varchar(10)");

        builder.Property(x => x.Descricao)
            .HasColumnType("varchar(300)");
    }
}