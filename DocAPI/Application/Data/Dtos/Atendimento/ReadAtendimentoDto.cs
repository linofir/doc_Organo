using DocAPI.Core.Entities;

namespace DocAPI.Data.Dtos.Atendimento;

public class ReadAtendimentoDto
{
    public Guid Id { get; set; }
    public Guid PacienteId { get; set; }
    public global::DocAPI.Core.Entities.Atendimento.EtapaAtendimento EtapaAtual { get; set; }
    public string? MensagemParaMedico { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}
