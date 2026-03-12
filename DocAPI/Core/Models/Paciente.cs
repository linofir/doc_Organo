using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.ConstrainedExecution;
using System.Text.Json.Serialization;

namespace DocAPI.Core.Models;

public class Paciente
{
    protected Paciente() {}
    public Paciente(string nome, DateOnly nascimento, string cpf, string rg, string email, string telefone) 
    {
        ID = Guid.NewGuid();
        Nome = nome;
        Nascimento = nascimento;
        CPF = cpf;
        Email = email;
        Telefone = telefone;
        
    }
    //acrescentar todas requireds no contructor

    public Guid ID { get; private set; } 
    public string Nome { get; private set; } = string.Empty;
    public DateOnly Nascimento { get; private set; }
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
    public string CPF { get; private set; } = string.Empty;
    public string? RG { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Telefone { get; private set; } = string.Empty;
    public string? Plano { get; private set; }
    public string? Carteira { get; private set; }
    public Endereco? Endereco { get; private set; } 

    public ICollection<Prontuario> Prontuarios { get; private set; } = new List<Prontuario>();
    public ICollection<Agendamento> Agendamentos { get; private set; } = new List<Agendamento>();
    public ICollection<Atendimento> Atendimentos { get; private set; } = new List<Atendimento>();
    
}