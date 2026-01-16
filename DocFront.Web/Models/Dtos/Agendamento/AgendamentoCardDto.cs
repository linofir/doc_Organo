namespace DocFront.Models.Dtos;

using DocFront.Models.Enums;

public class AgendamentoCardDto
{
    public DateOnly Data { get; set; } 
    public StatusAgendamento  Status { get; set; }

}