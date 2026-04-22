using DocAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
//Procura todas classes que implementam IEntityTypeConfiguration<>
public class PacienteConfig : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.ToTable("Paciente");

        builder.HasKey(x => x.ID);

        // =============================
        // Dados básicos
        // =============================
        builder.Property(x => x.Nome)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Nascimento)
            .IsRequired();

        builder.Property(x => x.CPF)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(x => x.RG)
            .HasMaxLength(20);

        builder.Property(x => x.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Telefone)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Plano)
            .HasMaxLength(100);

        builder.Property(x => x.Carteira)
            .HasMaxLength(100);

        // =============================
        // Auditoria
        // =============================
        builder.Property(x => x.CriadoEm)
            .IsRequired();

        builder.Property(x => x.AtualizadoEm);

        builder.Property(x => x.AtualizadoPor)
            .HasMaxLength(200);

        builder.Property(x => x.Deletado);

        builder.Property(x => x.DeletadoEm);

        // =============================
        // Endereço (Owned Entity)
        // =============================
        builder.OwnsOne(x => x.Endereco, endereco =>
        {
            endereco.Property(e => e.Logradouro)
                .HasColumnName("Endereco_Logradouro")
                .HasMaxLength(200);

            endereco.Property(e => e.Numero)
                .HasColumnName("Endereco_Numero")
                .HasMaxLength(50);

            endereco.Property(e => e.Bairro)
                .HasColumnName("Endereco_Bairro")
                .HasMaxLength(100);

            endereco.Property(e => e.Cidade)
                .HasColumnName("Endereco_Cidade")
                .HasMaxLength(100);

            endereco.Property(e => e.UF)
                .HasColumnName("Endereco_UF")
                .HasMaxLength(2)
                .IsFixedLength();

            endereco.Property(e => e.CEP)
                .HasColumnName("Endereco_CEP")
                .HasMaxLength(20);
        });

        // =============================
        // Índices
        // =============================
        builder.HasIndex(x => x.CPF)
            .IsUnique();

        builder.HasIndex(x => x.Email);
    }
}