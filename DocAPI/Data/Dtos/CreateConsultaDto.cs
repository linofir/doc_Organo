using System.ComponentModel.DataAnnotations;

namespace DocAPI.Data.Dtos;

public class CreateConsultaDto
{
    [Required(ErrorMessage = "A data é um valor necessário")]
    public DateTime Agendamento { get; set; }
    public string? Status { get; set; }
    [Required(ErrorMessage = "É necessário definir um Consultório")]
    public string? ConsultorioID { get; set; }
    [Required(ErrorMessage = "É necessário definir um Paciente")]
    public string? PacienteID { get; set; }
}