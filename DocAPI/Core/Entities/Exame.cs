namespace DocAPI.Core.Entities;

public class Exame
{
    protected Exame() {}

    public Exame(Guid prontuarioId, string codigo, string nome)
    {
        ID = Guid.NewGuid();
        ProntuarioId = prontuarioId;
        Codigo = codigo;
        Nome = nome;
    }

    public Guid ID { get; private set; }

    public Guid ProntuarioId { get; private set; }

    public Prontuario Prontuario { get; private set; } = null!;

    public string Codigo { get; private set; } = string.Empty;

    public string Nome { get; private set; } = string.Empty;

    public string? Status { get; private set; }

    public DateOnly? DataSolicitacao { get; private set; }

    public DateOnly? DataResultado { get; private set; }
}