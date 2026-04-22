using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;

public class ChecklistItemDefinitionConfiguration : IEntityTypeConfiguration<ChecklistItemDefinition>
{
    public void Configure(EntityTypeBuilder<ChecklistItemDefinition> builder)
    {
        builder.ToTable("ChecklistItemDefinition");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Descricao)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Ordem)
            .IsRequired();

        builder.Property(x => x.Obrigatorio)
            .IsRequired();

        builder.HasIndex(x => x.ChecklistDefinitionId);

        // 🔥 importante pra ordenação consistente
        builder.HasIndex(x => new { x.ChecklistDefinitionId, x.Ordem });
    }
}