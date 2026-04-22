namespace DocAPI.Core.Entities;

public class ItemFinanceiro
{
    protected ItemFinanceiro() { }

    public ItemFinanceiro(
        Guid guiaId,
        string lote,
        string guia,
        string protocolo,
        string nomeUsuario,
        string tipoUsuario,
        string info,
        DateOnly data,
        string servicoCodigo,
        string servicoDescricao,
        int? quantidade,
        decimal? valorTabela,
        string grauParticipacao,
        decimal? percentualVia,
        decimal? honorarioFator,
        decimal? valorPago)
    {
        Id = Guid.NewGuid();

        GuiaFinanceiraId = guiaId;

        Lote = lote;
        Guia = guia;
        Protocolo = protocolo;

        NomeUsuario = nomeUsuario;
        TipoUsuario = tipoUsuario;
        Info = info;

        Data = data;

        ServicoCodigo = servicoCodigo;
        ServicoDescricao = servicoDescricao;

        Quantidade = quantidade;
        ValorTabela = valorTabela;
        GrauParticipacao = grauParticipacao;
        PercentualVia = percentualVia;
        HonorarioFator = honorarioFator;
        ValorPago = valorPago;
    }

    public Guid Id { get; private set; }

    public Guid GuiaFinanceiraId { get; private set; }
    public GuiaFinanceira? GuiaFinanceira { get; private set; }

    public string? Lote { get; private set; }
    public string? Guia { get; private set; }
    public string? Protocolo { get; private set; }

    public string? NomeUsuario { get; private set; }

    public string? TipoUsuario { get; private set; }

    public string? Info { get; private set; }

    public DateOnly Data { get; private set; }

    public string? ServicoCodigo { get; private set; }
    public string? ServicoDescricao { get; private set; }

    public int? Quantidade { get; private set; }

    public decimal? ValorTabela { get; private set; }

    public string? GrauParticipacao { get; private set; }

    public decimal? PercentualVia { get; private set; }

    public decimal? HonorarioFator { get; private set; }

    public decimal? ValorPago { get; private set; }
    public DateTime CriadoEm { get; private set; }

}