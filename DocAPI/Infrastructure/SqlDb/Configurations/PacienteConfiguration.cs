using DocAPI.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
//Procura todas classes que implementam IEntityTypeConfiguration<>
public class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
{
    //a interface exige o método montar o modelo EF
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        //
        builder.HasKey(p => p.ID);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(p => p.Nascimento)
            .IsRequired();

        builder.Property(p => p.CPF)
            .HasMaxLength(11)
            .IsRequired();
        builder.HasIndex(p => p.CPF)
            .IsUnique();
            
        builder.Property(p => p.RG)
            .HasMaxLength(20);
        builder.HasIndex(p => p.RG)
            .IsUnique();

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(p => p.Telefone)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(p => p.Plano)
            .HasMaxLength(200);
        builder.Property(p => p.Carteira)
            .HasMaxLength(200);



        builder.Ignore(p => p.Idade);
        
        //Relaççoes
        builder.OwnsOne(p => p.Endereco);
  
        builder.HasMany(p => p.Prontuarios)
            .WithOne(p => p.Paciente)
            .HasForeignKey(p => p.PacienteId);
        // builder.HasMany(p => p.Agendamentos)
        //     .WithOne(p => p.Paciente)// eu referencio somente o ID, preciso mudar meu model para aceitar um ainstancia de paciente?
        //     .HasForeignKey(p => p.PacienteId);
        // builder.HasMany(p => p.Atendimentos)
        //     .WithOne(p => p.Paciente)// eu referencio somente o ID, preciso mudar meu model para aceitar um ainstancia de paciente?
        //     .HasForeignKey(p => p.PacienteId);
    }
}