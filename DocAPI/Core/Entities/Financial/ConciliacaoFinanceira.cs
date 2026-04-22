namespace DocAPI.Core.Entities;

public class ConciliacaoFinanceira
{
    protected ConciliacaoFinanceira() { }

    public ConciliacaoFinanceira(DateOnly periodo)
    {
        Id = Guid.NewGuid();
        Periodo = periodo;
        CriadoEm = DateTime.UtcNow;
        Itens = new List<ItemConciliacao>();
    }

    public Guid Id { get; private set; }

    public DateOnly Periodo { get; private set; }
    public  string? JsonOriginal { get; private set; }

    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }
    public string? AtualizadoPor { get; private set; }
    

    public ICollection<ItemConciliacao> Itens { get; private set; } = new List<ItemConciliacao>();
}