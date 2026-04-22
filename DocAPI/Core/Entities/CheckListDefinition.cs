namespace DocAPI.Core.Entities;

public class ChecklistDefinition
{
    protected ChecklistDefinition() { }

    public ChecklistDefinition(string nome, string? descricao)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Descricao = descricao;
    }

    public Guid Id { get; private set; }

    public string Nome { get; private set; } = string.Empty;

    public string? Descricao { get; private set; }

    public ICollection<ChecklistItemDefinition> Itens { get; private set; }
        = new List<ChecklistItemDefinition>();

    public void AdicionarItem(string descricao, int ordem, bool obrigatorio)
    {
        Itens.Add(new ChecklistItemDefinition(Id, descricao, ordem, obrigatorio));
    }
}