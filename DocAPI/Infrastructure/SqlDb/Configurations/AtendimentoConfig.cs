using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;

public class AtendimentoConfiguration : IEntityTypeConfiguration<Atendimento>
{
    public void Configure(EntityTypeBuilder<Atendimento> builder)
    {
        builder.ToTable("Atendimento");

        builder.HasKey(a => a.Id);

        // =============================
        // RELACIONAMENTO
        // =============================

        builder.HasOne(a => a.Paciente)
            .WithMany(p => p.Atendimentos)
            .HasForeignKey(a => a.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);

        // =============================
        // CAMPOS
        // =============================

        builder.Property(a => a.EtapaAtual)
            .HasConversion<string>() // 🔥 MUITO IMPORTANTE
            .HasMaxLength(50);

        builder.Property(a => a.MensagemParaMedico)
            .HasMaxLength(500);

        // =============================
        // AUDITORIA
        // =============================

        builder.Property(a => a.CriadoEm)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(a => a.AtualizadoEm)
            .HasColumnType("datetime2");

        builder.Property(a => a.AtualizadoPor)
            .HasMaxLength(200);

        builder.Property(a => a.Deletado);

        builder.Property(a => a.DeletadoEm)
            .HasColumnType("datetime2");

        // =============================
        // ÍNDICES
        // =============================

        builder.HasIndex(a => a.PacienteId);

        builder.HasIndex(a => a.EtapaAtual);

        // =============================
        // RELAÇÕES INTERNAS
        // =============================

        builder.HasMany(a => a.Eventos)
            .WithOne(e => e.Atendimento)
            .HasForeignKey(e => e.AtendimentoId);

        builder.HasMany(a => a.Pendencias)
            .WithOne(p => p.Atendimento)
            .HasForeignKey(p => p.AtendimentoId);

        builder.HasMany(a => a.Checklists)
            .WithOne(c => c.Atendimento)
            .HasForeignKey(c => c.AtendimentoId);
    }
}