namespace DocFront.Models.Dtos;

using DocFront.Models.Enums;

public class AgendamentoCardDto
{
    public string ID { get; set; } = string.Empty;
    public DateOnly Data { get; set; } 
    public StatusAgendamento  Status { get; set; }

}