namespace DocAPI.Core.Entities;

public class ChecklistItemDefinition
{
    protected ChecklistItemDefinition() { }

    public ChecklistItemDefinition(Guid checklistDefinitionId, string descricao, int ordem, bool obrigatorio)
    {
        Id = Guid.NewGuid();
        ChecklistDefinitionId = checklistDefinitionId;
        Descricao = descricao;
        Ordem = ordem;
        Obrigatorio = obrigatorio;
    }

    public Guid Id { get; private set; }

    public Guid ChecklistDefinitionId { get; private set; }

    public ChecklistDefinition ChecklistDefinition { get; private set; } = null!;

    public string Descricao { get; private set; } = string.Empty;

    public int Ordem { get; private set; }

    public bool Obrigatorio { get; private set; }
}