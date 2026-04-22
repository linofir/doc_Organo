using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DocAPI.Core.Entities;

public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("Agendamento");

        builder.HasKey(x => x.ID);

        builder.Property(x => x.Nome)
            .HasColumnType("varchar(20)");

        builder.Property(x => x.Aviso)
            .HasColumnType("varchar(20)");

        builder.Property(x => x.Local)
            .HasColumnType("varchar(100)");

        builder.Property(x => x.Sala)
            .HasColumnType("varchar(10)");

        builder.Property(x => x.Data)
            .HasColumnType("date");

        builder.Property(x => x.Horario)
            .HasColumnType("time");

        builder.Property(x => x.DataConsulta)
            .HasColumnType("date");

        // ENUMS como string
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasColumnType("varchar(30)");

        builder.Property(x => x.InstrucaoStatus)
            .HasConversion<string>()
            .HasColumnType("varchar(30)");

        builder.Property(x => x.AtestadoStatus)
            .HasConversion<string>()
            .HasColumnType("varchar(30)");

        // OWNED TYPE
        builder.OwnsOne(x => x.SenhaAgendamento, sa =>
        {
            sa.Property(p => p.Codigo)
                .HasColumnName("SenhaAgendamento_Codigo")
                .HasColumnType("varchar(20)");

            sa.Property(p => p.DataPedido)
                .HasColumnName("SenhaAgendamento_DataPedido")
                .HasColumnType("date");

            sa.Property(p => p.DataLiberacao)
                .HasColumnName("SenhaAgendamento_DataLiberacao")
                .HasColumnType("date");

            sa.Property(p => p.Validade)
                .HasColumnName("SenhaAgendamento_Validade")
                .HasColumnType("date");
        });

        // RELACIONAMENTOS
        builder.HasOne(x => x.Internacao)
            .WithMany()
            .HasForeignKey(x => x.InternacaoId);

        builder.HasOne(x => x.Atendimento)
            .WithMany()
            .HasForeignKey(x => x.AtendimentoId);

        // INDEXES
        builder.HasIndex(x => new { x.Data, x.Horario });
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.InternacaoId);
    }
}