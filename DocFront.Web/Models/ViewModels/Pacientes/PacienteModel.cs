
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocFront.Models;

public class PacienteModel
{
    public string ID { get; set; } = "";
    public string Nome { get; set; } = "";
    public string CPF { get; set; } = "";
    public string RG { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefone { get; set; } = "";
    public string Plano { get; set; } = "";
    public string Carteira { get; set; } = "";

    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? UF { get; set; }
    public string? CEP { get; set; }

    public DateOnly Nascimento { get; set; }

    public int Idade => 
        DateTime.Today.Year - Nascimento.Year 
        - (Nascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-((DateTime.Today.Year - Nascimento.Year)))) ? 1 : 0);
}