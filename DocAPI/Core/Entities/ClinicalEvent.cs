namespace DocAPI.Core.Entities;

public class ClinicalEvent
{
    protected ClinicalEvent() { }

    public ClinicalEvent(Guid atendimentoId, ClinicalEventType tipoEvento, string descricao, string usuario)
    {
        Id = Guid.NewGuid();
        AtendimentoId = atendimentoId;
        TipoEvento = tipoEvento;
        Descricao = descricao;
        Usuario = usuario;
        DataEvento = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid AtendimentoId { get; private set; }

    public Atendimento Atendimento { get; private set; } = null!;

    public ClinicalEventType TipoEvento { get; private set; }

    public string Descricao { get; private set; } = string.Empty;

    public DateTime DataEvento { get; private set; }

    public string Usuario { get; private set; } = string.Empty;
}
public enum ClinicalEventType
{
    ConsultaRealizada = 1,
    ExameSolicitado = 2,
    ExameResultadoRecebido = 3,
    InternacaoSolicitada = 4,
    InternacaoAutorizada = 5,
    ProcedimentoRealizado = 6,
    ConsultaPosOperatoria = 7,
    EtapaAlterada = 8
}


