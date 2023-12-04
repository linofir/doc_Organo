using DocAPI.Data.Dtos;

namespace DocAPI.Data.Dtos;

public class ReadConsultaDto
{
    public DateTime Agendamento { get; set; }
    public string? Status { get; set; }
    public ReadConsultorioDto? Consultorio { get; set; }
}