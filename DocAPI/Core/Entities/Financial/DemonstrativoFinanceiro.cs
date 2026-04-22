namespace DocAPI.Core.Entities;
public class DemonstrativoFinanceiro
{
    protected DemonstrativoFinanceiro() { }

    public DemonstrativoFinanceiro(DateOnly periodo)
    {
        Id = Guid.NewGuid();
        Periodo = periodo;
        Guias = new List<GuiaFinanceira>();
    }

    public Guid Id { get; private set; }

    public DateOnly Periodo { get; private set; }
    public string? JsonPath { get; private set; }

    public DateTime CriadoEm { get; private set; }


    public ICollection<GuiaFinanceira> Guias { get; private set; } = new List<GuiaFinanceira>();
}