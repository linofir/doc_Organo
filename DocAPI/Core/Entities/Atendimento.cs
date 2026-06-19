namespace DocAPI.Core.Entities;

public class Atendimento
{
    protected Atendimento() { }

    public Atendimento(Guid pacienteId, string? mensagemParaMedico = null)
    {
        Id = Guid.NewGuid();
        PacienteId = pacienteId;
        EtapaAtual = EtapaAtendimento.Consulta;
        MensagemParaMedico = mensagemParaMedico;
        CriadoEm = DateTime.UtcNow;
    }

    public void AtualizarMensagemParaMedico(string? mensagemParaMedico)
    {
        MensagemParaMedico = mensagemParaMedico;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void MarcarComoExcluido()
    {
        Deletado = true;
        DeletadoEm = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid PacienteId { get; private set; }

    public Paciente Paciente { get; private set; } = null!;

    public EtapaAtendimento EtapaAtual { get; private set; }

    public string? MensagemParaMedico { get; private set; }

    public ICollection<ClinicalEvent> Eventos { get; private set; } = new List<ClinicalEvent>();

    public ICollection<AtendimentoPendencia> Pendencias { get; private set; } = new List<AtendimentoPendencia>();

    public ICollection<ChecklistExecution> Checklists { get; private set; } = new List<ChecklistExecution>();


    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }
    public string? AtualizadoPor { get; private set; }

    public bool Deletado { get; private set; }
    public DateTime? DeletadoEm { get; private set; }


    public void AvancarEtapa()//como será esse gerenciamento
    {
        if (EtapaAtual == EtapaAtendimento.Finalizado)
            throw new InvalidOperationException("Atendimento já finalizado.");

        EtapaAtual++;
        RegistrarEvento(ClinicalEventType.EtapaAlterada, $"Etapa alterada para {EtapaAtual}", "médico");//ainda a ser implementado
    }

    public void RegistrarEvento(ClinicalEventType tipo, string descricao, string usuario)
    {
        Eventos.Add(new ClinicalEvent(Id, tipo, descricao, usuario));
    }

    public void AdicionarPendencia(string tipo, string descricao)
    {
        Pendencias.Add(new AtendimentoPendencia(Id, tipo, descricao));
    }
    public enum EtapaAtendimento
    {
        Consulta = 0,
        PreProcedimento = 1,
        Procedimento = 2,
        PosProcedimento = 3,
        Finalizado = 4
    }

}