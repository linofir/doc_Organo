namespace DocAPI.Core.Entities;

public class AtendimentoPendencia
{
    protected AtendimentoPendencia() { }

    public AtendimentoPendencia(Guid atendimentoId, string tipo, string descricao)
    {
        Id = Guid.NewGuid();
        AtendimentoId = atendimentoId;
        Tipo = tipo;
        Descricao = descricao;
        CriadoEm = DateTime.UtcNow;
        Resolvido = false;
    }

    public Guid Id { get; private set; }

    public Guid AtendimentoId { get; private set; }

    public Atendimento Atendimento { get; private set; } = null!;

    public string Tipo { get; private set; } = string.Empty;

    public string Descricao { get; private set; } = string.Empty;

    public bool Resolvido { get; private set; }

    public DateTime CriadoEm { get; private set; }

    public void Resolver()
    {
        Resolvido = true;
    }
}