using DocAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DocAPI.Data;

public class PacienteContext : DbContext
{
    public PacienteContext(DbContextOptions<PacienteContext> options)
        :base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Consulta>()
        .HasKey(consulta => new{consulta.PacienteID, consulta.ConsultorioID});

        builder.Entity<Consulta>()
        .HasOne(consulta => consulta.Paciente)
        .WithMany(paciente => paciente.Consultas)
        .HasForeignKey(consulta => consulta.PacienteID);

        builder.Entity<Consulta>()
        .HasOne(consulta => consulta.Local)
        .WithMany(consultorio => consultorio.Consultas)
        .HasForeignKey(consulta => consulta.ConsultorioID);

    }

    public DbSet<Paciente>? Pacientes { get; set;}
    public DbSet<Consulta>? Consultas { get; set;}
    public DbSet<Consultorio>? Consultorios { get; set;}
}