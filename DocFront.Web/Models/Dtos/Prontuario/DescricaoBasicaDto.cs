namespace DocFront.Models.Dtos;
public class DescricaoBasicaDto
{
    public string? PacienteId { get; set; }
    public string? NomePaciente { get; set; } 
    public string? Cpf { get; set; } 
    public int Idade { get; set; } 

    public string? Profissao { get; set; }

    public string? Religiao { get; set; }

    public string? QD { get; set; }
    public string? AtividadeFisica { get; set; }
}