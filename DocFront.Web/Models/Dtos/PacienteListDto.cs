namespace DocFront.Models.Dtos;

public class PacienteListDto
{
    public string Id { get; set; } = "";
    public string Nome { get; set; } = "";
    public string CPF { get; set; } = "";
    public string Email { get; set; } = "";
    public string Prontuarios { get; set; } = "Number";
    public string Agendamentos { get; set; } = "Status";
    public string Atendimento { get; set; } = "Status";

}