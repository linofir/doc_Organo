namespace DocAPI.Core.Entities;

public class ProcedimentoInternacao
{
    protected ProcedimentoInternacao(){}

    public ProcedimentoInternacao(Guid internacaoId, string codigo, string descricao)
    {
        ID = Guid.NewGuid();
        InternacaoId = internacaoId;
        CodigoProcedimento = codigo;
        Descricao = descricao;
    }

    public Guid ID { get; private set; }

    public Guid InternacaoId { get; private set; }

    public Internacao Internacao { get; private set; } = null!;

    public string CodigoProcedimento { get; private set; } = string.Empty;

    public string Descricao { get; private set; } = string.Empty;
}