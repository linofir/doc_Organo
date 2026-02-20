using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.ConstrainedExecution;
using System.Text.Json.Serialization;

namespace DocAPI.Core.Models;

public class Paciente
{
    protected Paciente() {}
    public Paciente(string nome, DateOnly nascimento, string cpf) // esse contructor seria utilizado nos repositórios por exemplo?Isso seria útil caso meu rep tenha q instanciar ma paciente correto?posso organizar meu código sem necessáriamente utilizar o constructor, correto? qual é a forma mais profissional. me explique o conceito de SOLID
    {
        ID = Guid.NewGuid();
        Nome = nome;
        Nascimento = nascimento;
        CPF = cpf;
    }

    public Guid ID { get; private set; } 
    public string? Nome { get; private set; }
    public DateOnly Nascimento { get; private set; }
    [NotMapped] 
    public int Idade
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var idade = today.Year - Nascimento.Year;
            if (Nascimento > today.AddYears(-idade)) idade--;
            return idade;
        }
    }
    public string? CPF { get; private set; }
    public string? RG { get; private set; }
    public string? Email { get; private set; }
    public string? Telefone { get; private set; }
    public string? Plano { get; private set; }// possível enum
    public string? Carteira { get; private set; }
    public Endereco? Endereco { get; private set; } 

    public ICollection<Prontuario> Prontuarios { get; private set; } = new List<Prontuario>();
    public ICollection<Agendamento> Agendamentos { get; private set; } = new List<Agendamento>();
    public ICollection<Atendimento> Atendimentos { get; private set; } = new List<Atendimento>();
    
}