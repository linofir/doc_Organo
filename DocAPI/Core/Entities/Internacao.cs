namespace DocAPI.Core.Entities;

public class Internacao
{
    protected Internacao(){}

    public Internacao(Guid prontuarioId)
    {
        ID = Guid.NewGuid();
        ProntuarioId = prontuarioId;
    }

    public Guid ID { get; private set; }

    public Guid ProntuarioId { get; private set; }

    public Prontuario Prontuario { get; private set; } = null!;

    public DateOnly Data { get; private set; }

    public string IndicacaoClinica { get; private set; } = string.Empty;

    public string Observacao { get; private set; } = string.Empty;

    public string CIDCodigo { get; private set; } = string.Empty;

    public CID CID { get; private set; } = null!;

    public string TempoDoenca { get; private set; } = string.Empty;

    public int Diarias { get; private set; }

    public string Tipo { get; private set; } = string.Empty;

    public string Regime { get; private set; } = string.Empty;

    public string Carater { get; private set; } = string.Empty;

    public bool UsaOPME { get; private set; }

    public string Local { get; private set; } = string.Empty;

    public string? Guia { get; private set; }

    public ICollection<ProcedimentoInternacao> Procedimentos { get; private set; }
        = new List<ProcedimentoInternacao>();
}