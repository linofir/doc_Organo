using DocAPI.Core.Enums;
namespace DocAPI.Core.Entities;
public class ItemConciliacao
{
    protected ItemConciliacao() { }

    public ItemConciliacao(
        Guid conciliacaoId,
        Guid itemFinanceiroId,
        decimal valorEsperado,
        decimal valorPago)
    {
        Id = Guid.NewGuid();

        ConciliacaoId = conciliacaoId;
        ItemFinanceiroId = itemFinanceiroId;

        ValorEsperado = valorEsperado;
        ValorPago = valorPago;

        Diferenca = valorPago - valorEsperado;

        Status = CalcularStatus();
        CriadoEm = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid ConciliacaoId { get; private set; }
    public ConciliacaoFinanceira? Conciliacao { get; private set; }

    public Guid ItemFinanceiroId { get; private set; }
    public ItemFinanceiro? ItemFinanceiro { get; private set; }

    public Guid? AtendimentoId { get; private set; }
    public Guid? ProcedimentoInternacaoId { get; private set; }

    public decimal ValorEsperado { get; private set; }
    public decimal ValorPago { get; private set; }
    public decimal Diferenca { get; private set; }

    public StatusConciliacao Status { get; private set; }

    public string? Observacao { get; private set; }

    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }
    public string? AtualizadoPor { get; private set; }



    private StatusConciliacao CalcularStatus()
    {
        if (ValorEsperado == 0)
            return StatusConciliacao.NaoEncontrado;

        if (ValorEsperado == ValorPago)
            return StatusConciliacao.Conciliado;

        return StatusConciliacao.Divergente;
    }
}