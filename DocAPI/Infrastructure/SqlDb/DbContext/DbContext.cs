using Microsoft.EntityFrameworkCore;
using DocAPI.Core.Entities;
using DocAPI.Infrastructure.SqlDb.Audit;

namespace DocAPI.Infrastructure.SqlDb.Context;
public class DocDbContext : DbContext
{
    public DocDbContext(DbContextOptions<DocDbContext> options)
        : base(options) { }

    // =============================
    // AGGREGATE ROOTS
    // =============================
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Atendimento> Atendimentos => Set<Atendimento>();
    public DbSet<Prontuario> Prontuarios => Set<Prontuario>();
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();

    // =============================
    // ENTIDADES CONSULTÁVEIS IMPORTANTES
    // =============================
    public DbSet<ClinicalEvent> ClinicalEvents => Set<ClinicalEvent>();
    public DbSet<AtendimentoPendencia> AtendimentoPendencias => Set<AtendimentoPendencia>();

    public DbSet<ChecklistDefinition> ChecklistDefinitions => Set<ChecklistDefinition>();
    public DbSet<ChecklistExecution> ChecklistExecutions => Set<ChecklistExecution>();

    public DbSet<ProntuarioAcaoCD> ProntuarioAcoesCD => Set<ProntuarioAcaoCD>();

    public DbSet<CID> CIDs => Set<CID>();

    // =============================
    // FINANCIAL
    // =============================
    public DbSet<DemonstrativoFinanceiro> DemonstrativosFinanceiros => Set<DemonstrativoFinanceiro>();
    public DbSet<GuiaFinanceira> GuiasFinanceiras => Set<GuiaFinanceira>();
    public DbSet<ItemFinanceiro> ItensFinanceiros => Set<ItemFinanceiro>();

    public DbSet<ConciliacaoFinanceira> Conciliacoes => Set<ConciliacaoFinanceira>();
    public DbSet<ItemConciliacao> ConciliacaoItens => Set<ItemConciliacao>();

    // Auditoria (opcional via EF)
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas as configurações Fluent API
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DocDbContext).Assembly);

        // =============================
        // CONFIGURAÇÕES GLOBAIS IMPORTANTES
        // =============================

        // Soft Delete global filter
        ApplySoftDeleteQueryFilter(modelBuilder);

        // Padronização de string
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var prop in entity.GetProperties())
            {
                if (prop.ClrType == typeof(string))
                {
                    // prop.SetIsUnicode(false); // opcional (performance)
                }
            }
        }

        base.OnModelCreating(modelBuilder);
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<DateOnly>()
            .HaveConversion<DateOnlyConverter>()
            .HaveColumnType("date");

        builder.Properties<TimeOnly>()
            .HaveConversion<TimeOnlyConverter>()
            .HaveColumnType("time");
    }

    private void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Paciente>().HasQueryFilter(p => !p.Deletado);
        modelBuilder.Entity<Atendimento>().HasQueryFilter(a => !a.Deletado);
        modelBuilder.Entity<Prontuario>().HasQueryFilter(p => !p.Deletado);
        modelBuilder.Entity<Agendamento>().HasQueryFilter(a => !a.Deletado);
    }
}