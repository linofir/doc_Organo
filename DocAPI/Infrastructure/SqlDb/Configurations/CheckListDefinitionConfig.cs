using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;

public class ChecklistDefinitionConfiguration : IEntityTypeConfiguration<ChecklistDefinition>
{
    public void Configure(EntityTypeBuilder<ChecklistDefinition> builder)
    {
        builder.ToTable("ChecklistDefinition");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Descricao)
            .HasMaxLength(200);

        builder.HasMany(x => x.Itens)
            .WithOne(i => i.ChecklistDefinition)
            .HasForeignKey(i => i.ChecklistDefinitionId);
    }
}