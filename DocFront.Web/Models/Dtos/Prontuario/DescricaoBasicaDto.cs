namespace DocFront.Models.Dtos;
public class DescricaoBasicaDto
{
    public string PacienteId { get; set; } = string.Empty;
    public string NomePaciente { get; set; } = string.Empty; 
    public string Cpf { get; set; } = string.Empty; 
    public int Idade { get; set; } 

    public string Profissao { get; set; } = string.Empty;

    public string Religiao { get; set; } = string.Empty;

    public string QD { get; set; } = string.Empty;
    public string AtividadeFisica { get; set; } = string.Empty;
}