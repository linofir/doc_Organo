using Microsoft.EntityFrameworkCore;
using DocAPI.Core.Models;

namespace DocAPI.Infrastructure.SqlDb;

public class DocDbContext : DbContext
{
    public DocDbContext(DbContextOptions<DocDbContext> options)
        : base(options) { }

    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Prontuario> Prontuarios => Set<Prontuario>();
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();
    public DbSet<Atendimento> Atendimentos => Set<Atendimento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DocDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}