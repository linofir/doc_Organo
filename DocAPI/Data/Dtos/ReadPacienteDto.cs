using System.Text.Json.Serialization;
using DocAPI.Core.Models;

namespace DocAPI.Data.Dtos;

public class ReadPacienteDto
{
    public string ID { get; set; }
    public string Nome { get; set; }
    public DateTime Nascimento { get; set; }
    public int Idade
    {
        get
        {
            var today = DateTime.Today;
            var idade = today.Year - Nascimento.Year;
            if (Nascimento.Date > today.AddYears(-idade)) idade--;
            return idade;
        }
    }
    public string CPF { get; set; }
    public string? RG { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public string Plano { get; set; }
    public string Carteira { get; set; }
    public ReadEnderecoDto Endereco { get; set; } = new();
    public DateTime HorarioDaAcao { get; set; } = DateTime.Now;
    // public ICollection<ReadConsultaDto>? Consultas { get; set; }
    
}