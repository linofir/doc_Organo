namespace DocAPI.Core.Entities;

public class ChecklistItemExecution
{
    protected ChecklistItemExecution() { }

    public ChecklistItemExecution(Guid checklistExecutionId, Guid checklistItemDefinitionId)
    {
        Id = Guid.NewGuid();
        ChecklistExecutionId = checklistExecutionId;
        ChecklistItemDefinitionId = checklistItemDefinitionId;
    }

    public Guid Id { get; private set; }

    public Guid ChecklistExecutionId { get; private set; }

    public ChecklistExecution ChecklistExecution { get; private set; } = null!;

    public Guid ChecklistItemDefinitionId { get; private set; }

    public ChecklistItemDefinition ChecklistItemDefinition { get; private set; } = null!;

    public bool Concluido { get; private set; }

    public DateTime? ConcluidoEm { get; private set; }

    public string? ConcluidoPor { get; private set; }

    public void Concluir(string usuario)
    {
        Concluido = true;
        ConcluidoPor = usuario;
        ConcluidoEm = DateTime.UtcNow;
    }
}