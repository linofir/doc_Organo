using System.Text.Json.Serialization;

namespace DocAPI.Data.Dtos;

public class ReadPacienteDto
{
    public string? Nome { get; set; }
    public string? Nascimento { get; set; }
    public int Idade { get; set; }
    public string? CPF { get; set; }
    public string? RG { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public DateTime HorarioDaAcao { get; set; } = DateTime.Now;
    // public ICollection<ReadConsultaDto>? Consultas { get; set; }
    
}