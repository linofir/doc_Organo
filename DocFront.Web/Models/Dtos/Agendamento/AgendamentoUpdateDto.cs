namespace DocFront.Models.Dtos;

using DocFront.Models.Enums;
public class AgendamentoUpdateApiDto
{
    public string PacienteId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Aviso { get; set; } = string.Empty;
    public DateOnly Data { get; set; }
    public TimeOnly Horario { get; set; }
    public string Procedimento { get; set; } = string.Empty;
    public string Local { get; set; } = string.Empty;
    public string? Sala { get; set; }
    public SenhaDto? SenhaAgendamento { get; set; }
    public StatusAgendamento Status { get; set; }
    public string? StatusInstrucoes { get; set; }
    public string? StatusAtestado { get; set; }
}
