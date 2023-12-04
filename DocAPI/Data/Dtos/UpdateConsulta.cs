using System.ComponentModel.DataAnnotations;

namespace DocAPI.Data.Dtos;

public class UpdateConsultaDto
{
    [Required(ErrorMessage = "A data é um valor necessário")]
    public DateTime Agendamento { get; set; }
    public string? Status { get; set; }
    public string? ConsultorioID { get; set; }
}