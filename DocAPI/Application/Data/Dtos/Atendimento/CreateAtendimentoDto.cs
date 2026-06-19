namespace DocAPI.Data.Dtos.Atendimento;

public class CreateAtendimentoDto
{
    public Guid PacienteId { get; set; }
    public string? MensagemParaMedico { get; set; }
}
