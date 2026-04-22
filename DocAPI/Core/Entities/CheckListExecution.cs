namespace DocAPI.Core.Entities;

public class ChecklistExecution
{
    protected ChecklistExecution() { }

    public ChecklistExecution(Guid atendimentoId, Guid checklistDefinitionId)
    {
        Id = Guid.NewGuid();
        AtendimentoId = atendimentoId;
        ChecklistDefinitionId = checklistDefinitionId;
        CriadoEm = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid ChecklistDefinitionId { get; private set; }

    public ChecklistDefinition ChecklistDefinition { get; private set; } = null!;

    public Guid AtendimentoId { get; private set; }

    public Atendimento Atendimento { get; private set; } = null!;

    public DateTime CriadoEm { get; private set; }

    public ICollection<ChecklistItemExecution> Itens { get; private set; }
        = new List<ChecklistItemExecution>();
}