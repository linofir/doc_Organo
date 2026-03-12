using Microsoft.EntityFrameworkCore;
using DocAPI.Core.Models;

namespace DocAPI.Infrastructure.SqlDb.Context;
//hereança  de DbContext
public class DocDbContext : DbContext
{
    //chama o construtor da classe pai para ID: de connection, provider, configs
    public DocDbContext(DbContextOptions<DocDbContext> options)
        : base(options) { }

    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Prontuario> Prontuarios => Set<Prontuario>();
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();
    public DbSet<Atendimento> Atendimentos => Set<Atendimento>();
    //Procura todas classes que implementam automaticamente IEntityTypeConfiguration<>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DocDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}