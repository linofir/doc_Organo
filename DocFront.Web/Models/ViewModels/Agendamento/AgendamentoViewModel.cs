using DocFront.Models.Enums;
using DocFront.Models.ViewModels;
using DocFront.Utils;


namespace DocFront.Models.ViewModels;

public class AgendamentoViewModel
{
    public string ID { get; set; } = string.Empty;
    public string PacienteID { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Aviso { get; set; } = string.Empty;
    public DateOnly Data { get; set; } = DateOnly.MinValue;
    public TimeOnly Horario { get; set; }
    public string HorarioTexto
    {
        get => Horario.ToString("HH:mm");
        set
        {
            if (!TimeOnly.TryParse(value, out var time))
            return; 
            Horario = time;
        }
    }
    public string Procedimento { get; set; } = string.Empty;
    public string Local { get; set; } = string.Empty;
    public string Sala { get; set; } =string.Empty;
    public SenhaViewModel? SenhaAgendamento { get; set; }
    public StatusAgendamento  Status { get; set; } = StatusAgendamento.SemSenha;
    public string? StatusInstrucoes { get; set; }
    public string? StatusAtestado { get; set; }
    public DateOnly DataConsulta { get; set; } = DateOnly.MinValue;   
  
}