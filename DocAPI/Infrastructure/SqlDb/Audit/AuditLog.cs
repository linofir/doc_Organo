namespace DocAPI.Infrastructure.SqlDb.Audit;

public class AuditLog
{
    public Guid ID { get; set; }

    public string Tabela { get; set; } = string.Empty;

    public Guid RegistroId { get; set; }

    public AuditOperacao Operacao { get; set; }

    public string? ValorAnterior { get; set; }

    public string? ValorNovo { get; set; }

    public string Usuario { get; set; } = string.Empty;

    public DateTime DataOperacao { get; set; }
}

public enum AuditOperacao
{
    Insert = 0,
    Update = 1,
    Delete = 2
}