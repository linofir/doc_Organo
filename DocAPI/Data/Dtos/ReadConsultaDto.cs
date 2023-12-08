using DocAPI.Data.Dtos;

namespace DocAPI.Data.Dtos;

public class ReadConsultaDto
{
    public DateTime Agendamento { get; set; }
    public string? Status { get; set; }
    public string? PacienteID { get; set; }
    public string? ConsultorioID { get; set; }
    public ReadConsultorioDto? Consultorio { get; set; }
    public ReadPacienteDto? Paciente { get; set; }
}