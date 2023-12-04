using DocAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DocAPI.Data;

public class PacienteContext : DbContext
{
    public PacienteContext(DbContextOptions<PacienteContext> options)
        :base(options)
    {
        
    }

    public DbSet<Paciente>? Pacientes { get; set;}
    public DbSet<Consulta>? Consultas { get; set;}
    public DbSet<Consultorio>? Consultorios { get; set;}
}