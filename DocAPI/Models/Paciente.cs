using System.Runtime.ConstrainedExecution;

namespace DocAPI.Models;

public class Paciente
{
    private string? ID { get; set; }
    public string? Nome { get; set; }
    public DateTime Nascimento { get; set; }
    public int Idade { get; set; }
    public int CPF { get; set; }
    public int RG { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    
    public Paciente(string? nome, DateTime nascimento, int idade, int cpf, int rg, string? email, string? telefone, int myProperty)
    {
        ID = Guid.NewGuid().ToString();
        Nome = nome;
        Nascimento = nascimento;
        Idade = idade;
        CPF = cpf;
        RG = rg;
        Email = email;
        Telefone = telefone;

    }
}