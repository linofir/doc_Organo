namespace DocFront.Models.Dtos;

using DocFront.Models.Enums;

public class AgendamentoReadApiDto
{
    public string? Id { get; set; }  
    public string? PacienteID { get; set; }
    public string? Nome { get; set; }
    public string? Aviso { get; set; }
    public DateOnly Data { get; set; } 
    public TimeOnly Horario { get; set; }
    public string? Procedimento { get; set; }
    public string? Local { get; set; }
    public string? Sala { get; set; }
    public SenhaDto? SenhaAgendamento { get; set; }
    public StatusAgendamento  Status { get; set; }
    public string? StatusInstrucoes { get; set; }
    public string? StatusAtestado { get; set; }
    public DateOnly DataConsulta { get; set; }     
}