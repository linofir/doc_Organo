// using DocAPI.Core.Enums;

namespace DocAPI.Core.Entities;
public class GuiaFinanceira
{
    protected GuiaFinanceira() { }

    public GuiaFinanceira(Guid demonstrativoId, string tipoGuia)
    {
        Id = Guid.NewGuid();
        DemonstrativoFinanceiroId = demonstrativoId;
        TipoGuia = tipoGuia;
        Itens = new List<ItemFinanceiro>();
    }

    public Guid Id { get; private set; }

    public Guid DemonstrativoFinanceiroId { get; private set; }

    public string? TipoGuia { get; private set; }

    public string? TipoGuiaDescricao { get; private set; }

    public DemonstrativoFinanceiro? DemonstrativoFinanceiro { get; private set; }
    public DateTime CriadoEm { get; private set; }


    public ICollection<ItemFinanceiro> Itens { get; private set; } =  new List<ItemFinanceiro>();
}