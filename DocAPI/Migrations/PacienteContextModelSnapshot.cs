﻿// <auto-generated />
using System;
using DocAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DocAPI.Migrations
{
    [DbContext(typeof(PacienteContext))]
    partial class PacienteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("DocAPI.Models.Consulta", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("Agendamento")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ConsultorioID")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PacienteID")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Status")
                        .HasColumnType("longtext");

                    b.HasKey("ID");

                    b.HasIndex("ConsultorioID");

                    b.HasIndex("PacienteID");

                    b.ToTable("Consultas");
                });

            modelBuilder.Entity("DocAPI.Models.Consultorio", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Complemento")
                        .HasColumnType("longtext");

                    b.Property<string>("Logradouro")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Numero")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("Consultorios");
                });

            modelBuilder.Entity("DocAPI.Models.Paciente", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("CPF")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("varchar(11)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Idade")
                        .HasColumnType("int");

                    b.Property<string>("Nascimento")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RG")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("varchar(11)");

                    b.Property<string>("Telefone")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("ID");

                    b.ToTable("Pacientes");
                });

            modelBuilder.Entity("DocAPI.Models.Consulta", b =>
                {
                    b.HasOne("DocAPI.Models.Consultorio", "Local")
                        .WithMany("Consultas")
                        .HasForeignKey("ConsultorioID");

                    b.HasOne("DocAPI.Models.Paciente", "Paciente")
                        .WithMany("Consultas")
                        .HasForeignKey("PacienteID");

                    b.Navigation("Local");

                    b.Navigation("Paciente");
                });

            modelBuilder.Entity("DocAPI.Models.Consultorio", b =>
                {
                    b.Navigation("Consultas");
                });

            modelBuilder.Entity("DocAPI.Models.Paciente", b =>
                {
                    b.Navigation("Consultas");
                });
#pragma warning restore 612, 618
        }
    }
}
