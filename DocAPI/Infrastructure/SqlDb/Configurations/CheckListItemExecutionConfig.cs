using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;

public class ChecklistItemExecutionConfiguration : IEntityTypeConfiguration<ChecklistItemExecution>
{
    public void Configure(EntityTypeBuilder<ChecklistItemExecution> builder)
    {
        builder.ToTable("ChecklistItemExecution");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ConcluidoPor)
            .HasMaxLength(50);

        builder.Property(x => x.ConcluidoEm)
            .HasColumnType("datetime2");

        builder.HasOne(x => x.ChecklistExecution)
            .WithMany(e => e.Itens)
            .HasForeignKey(x => x.ChecklistExecutionId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.HasOne(x => x.ChecklistItemDefinition)
            .WithMany()
            .HasForeignKey(x => x.ChecklistItemDefinitionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.ChecklistExecutionId);

        builder.HasIndex(x => x.ChecklistItemDefinitionId);

        // 🔥 ESSENCIAL - evita duplicação do mesmo item na execução
        builder.HasIndex(x => new { x.ChecklistExecutionId, x.ChecklistItemDefinitionId })
            .IsUnique();
    }
}