using DocAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
//Procura todas classes que implementam IEntityTypeConfiguration<>
public class ProntuarioConfiguration : IEntityTypeConfiguration<Prontuario>
{
    public void Configure(EntityTypeBuilder<Prontuario> builder)
    {
        builder.ToTable("Prontuario");

        builder.HasKey(p => p.ID);

        // =============================
        // RELACIONAMENTOS
        // =============================

        builder.HasOne(p => p.Paciente)
            .WithMany(p => p.Prontuarios)
            .HasForeignKey(p => p.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Atendimento)
            .WithMany()
            .HasForeignKey(p => p.AtendimentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ProntuarioAnterior)
            .WithMany()
            .HasForeignKey(p => p.ProntuarioAnteriorId)
            .OnDelete(DeleteBehavior.Restrict);

        // =============================
        // CAMPOS SIMPLES
        // =============================

        builder.Property(p => p.DataConsulta)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(p => p.Tipo);

        builder.Property(p => p.InformacoesExtras)
            .HasColumnType("varchar(max)");

        builder.Property(p => p.Versao)
            .IsRequired();

        // =============================
        // AUDITORIA
        // =============================

        builder.Property(p => p.CriadoEm)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(p => p.AtualizadoEm)
            .HasColumnType("datetime2");

        builder.Property(p => p.AtualizadoPor)
            .HasMaxLength(200);

        builder.Property(p => p.Deletado);

        builder.Property(p => p.DeletadoEm)
            .HasColumnType("datetime2");

        // =============================
        // ÍNDICES IMPORTANTES
        // =============================

        builder.HasIndex(p => p.PacienteId);

        builder.HasIndex(p => p.AtendimentoId);

        builder.HasIndex(p => p.ProntuarioAnteriorId);

        builder.HasIndex(p => new { p.PacienteId, p.Versao })
            .IsUnique();

        // =============================
        // VALUE OBJECTS (OWNED TYPES)
        // =============================

        builder.OwnsOne(p => p.DescricaoBasica, db =>
        {
            db.Property(x => x.NomePaciente).HasColumnName("DescricaoBasica_NomePaciente").HasMaxLength(200);
            db.Property(x => x.Cpf).HasColumnName("DescricaoBasica_Cpf").HasMaxLength(11);
            db.Property(x => x.Idade).HasColumnName("DescricaoBasica_Idade");
            db.Property(x => x.Profissao).HasColumnName("DescricaoBasica_Profissao").HasMaxLength(200);
            db.Property(x => x.Religiao).HasColumnName("DescricaoBasica_Religiao").HasMaxLength(200);
            db.Property(x => x.QD).HasColumnName("DescricaoBasica_QD").HasColumnType("varchar(max)");
            db.Property(x => x.AtividadeFisica).HasColumnName("DescricaoBasica_AtividadeFisica").HasMaxLength(200);
        });

        builder.OwnsOne(p => p.AGO, ago =>
        {
            ago.Property(x => x.Menarca).HasColumnName("Ago_Menarca").HasMaxLength(50);
            ago.Property(x => x.DUM).HasColumnName("Ago_DUM").HasMaxLength(50);
            ago.Property(x => x.Paridade).HasColumnName("Ago_Paridade").HasMaxLength(50);
            ago.Property(x => x.DesejoGestacao).HasColumnName("Ago_DesejoGestacao").HasMaxLength(50);

            ago.Property(x => x.VacinaHPV)
                .HasColumnName("Ago_VacinaHPV");

            ago.Property(x => x.CCO).HasColumnName("Ago_Cco").HasColumnType("varchar(max)");
            ago.Property(x => x.MAC_TRH).HasColumnName("Ago_MAC_TRH").HasMaxLength(100);

            ago.Property(x => x.Intercorrencias).HasColumnName("Ago_Intercorrencias").HasMaxLength(200);
            ago.Property(x => x.Amamentacao).HasColumnName("Ago_Amamentacao").HasMaxLength(100);
            ago.Property(x => x.VidaSexual).HasColumnName("Ago_VidaSexual").HasMaxLength(100);
            ago.Property(x => x.Relacionamento).HasColumnName("Ago_Relacionamento").HasMaxLength(100);
            ago.Property(x => x.Parceiros).HasColumnName("Ago_Parceiros").HasMaxLength(100);
            ago.Property(x => x.Coitarca).HasColumnName("Ago_Coitarca").HasMaxLength(100);
            ago.Property(x => x.IST).HasColumnName("Ago_IST").HasMaxLength(100);
        });

        builder.OwnsOne(p => p.Antecedentes, ant =>
        {
            ant.Property(x => x.Comorbidades).HasColumnName("Antecedentes_Comorbidades").HasMaxLength(100);
            ant.Property(x => x.Medicacao).HasColumnName("Antecedentes_Medicacao").HasMaxLength(100);
            ant.Property(x => x.Neoplasias).HasColumnName("Antecedentes_Neoplasias").HasMaxLength(100);
            ant.Property(x => x.Cirurgias).HasColumnName("Antecedentes_Cirurgias").HasMaxLength(100);
            ant.Property(x => x.Alergias).HasColumnName("Antecedentes_Alergias").HasMaxLength(100);
            ant.Property(x => x.Vicios).HasColumnName("Antecedentes_Vicios").HasMaxLength(100);
            ant.Property(x => x.HabitoIntestinal).HasColumnName("Antecedentes_HabitoIntestinal").HasMaxLength(200);
            ant.Property(x => x.Vacinas).HasColumnName("Antecedentes_Vacinas").HasMaxLength(200);
        });

        builder.OwnsOne(p => p.AntecedentesFamiliares, fam =>
        {
            fam.Property(x => x.Neoplasias).HasColumnName("AntecedentesFamiliares_Neoplasias").HasMaxLength(200);
            fam.Property(x => x.Comorbidades).HasColumnName("AntecedentesFamiliares_Comorbidades").HasMaxLength(200);
        });

        builder.OwnsOne(p => p.PosOperatorio, pos =>
        {
            pos.Property(x => x.PeriodoSeguimento).HasColumnName("PosOp_PeriodoSeguimento").HasMaxLength(100);
            pos.Property(x => x.Conclusao).HasColumnName("PosOp_Conclusao").HasMaxLength(500);
            pos.Property(x => x.ExameMacro).HasColumnName("PosOp_ExameMacro").HasColumnType("varchar(max)");
        });

        // =============================
        // COLEÇÕES
        // =============================

        builder.HasMany(p => p.AcoesCD)
            .WithOne(a => a.Prontuario)
            .HasForeignKey(a => a.ProntuarioId);
    }
}