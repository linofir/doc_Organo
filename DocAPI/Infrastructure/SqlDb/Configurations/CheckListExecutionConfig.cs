using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;
public class ChecklistExecutionConfiguration : IEntityTypeConfiguration<ChecklistExecution>
{
    public void Configure(EntityTypeBuilder<ChecklistExecution> builder)
    {
        builder.ToTable("ChecklistExecution");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CriadoEm)
            .HasColumnType("datetime2");

        builder.HasOne(x => x.Atendimento)
            .WithMany(a => a.Checklists)
            .HasForeignKey(x => x.AtendimentoId);

        builder.HasOne(x => x.ChecklistDefinition)
            .WithMany()
            .HasForeignKey(x => x.ChecklistDefinitionId);

        builder.HasIndex(x => new { x.AtendimentoId, x.ChecklistDefinitionId })
            .IsUnique();
    }
}