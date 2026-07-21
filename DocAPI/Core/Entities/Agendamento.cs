using System.ComponentModel.DataAnnotations;

namespace DocAPI.Core.Entities;

public class Agendamento
{
    protected Agendamento() { }

    public Agendamento(
        Guid internacaoId,
        Guid atendimentoId,
        DateOnly data,
        TimeOnly horario,
        Guid? pacienteID = null,
        string? nome = null,
        string? aviso = null,
        string? local = null,
        string? sala = null,
        DateOnly? dataConsulta = null,
        StatusInstrucoes instrucaoStatus = StatusInstrucoes.SemSolicitação,
        StatusAtestado atestadoStatus = StatusAtestado.NaoRealizado,
        SenhaAgendamento? senhaAgendamento = null)
    {
        ID = Guid.NewGuid();
        InternacaoId = internacaoId;
        AtendimentoId = atendimentoId;
        Data = data;
        Horario = horario;
        PacienteID = pacienteID;
        Nome = nome ?? string.Empty;
        Aviso = aviso ?? string.Empty;
        Local = local ?? string.Empty;
        Sala = sala ?? string.Empty;
        DataConsulta = dataConsulta ?? DateOnly.MinValue;
        InstrucaoStatus = instrucaoStatus;
        AtestadoStatus = atestadoStatus;
        SenhaAgendamento = senhaAgendamento;
        CriadoEm = DateTime.UtcNow;
        Status = StatusAgendamento.SemSenha;
    }

    public Guid ID { get; private set; }

    public Guid InternacaoId { get; private set; }
    public Internacao Internacao { get; private set; } = null!;

    public Guid AtendimentoId { get; private set; }
    public Atendimento Atendimento { get; private set; } = null!;

    public Guid? PacienteID { get; private set; }

    public string Nome { get; private set; }

    public string Aviso { get; private set; }

    public DateOnly Data { get; private set; }

    public TimeOnly Horario { get; private set; }

    public string Local { get; private set; }

    public string Sala { get; private set; }

    public StatusAgendamento Status { get; private set; }

    public StatusInstrucoes InstrucaoStatus { get; private set; }

    public StatusAtestado AtestadoStatus { get; private set; }

    public DateOnly DataConsulta { get; private set; }

    public SenhaAgendamento? SenhaAgendamento { get; private set; }

    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }
    public string? AtualizadoPor { get; private set; }

    public bool Deletado { get; private set; }
    public DateTime? DeletadoEm { get; private set; }

    public void SoftDelete()
    {
        Deletado = true;
        DeletadoEm = DateTime.UtcNow;
    }

    public void Update(
        string? nome = null,
        string? aviso = null,
        DateOnly? data = null,
        TimeOnly? horario = null,
        string? local = null,
        string? sala = null,
        StatusAgendamento? status = null,
        StatusInstrucoes? instrucaoStatus = null,
        StatusAtestado? atestadoStatus = null,
        DateOnly? dataConsulta = null)
    {
        if (nome is not null) Nome = nome;
        if (aviso is not null) Aviso = aviso;
        if (data.HasValue) Data = data.Value;
        if (horario.HasValue) Horario = horario.Value;
        if (local is not null) Local = local;
        if (sala is not null) Sala = sala;
        if (status.HasValue) Status = status.Value;
        if (instrucaoStatus.HasValue) InstrucaoStatus = instrucaoStatus.Value;
        if (atestadoStatus.HasValue) AtestadoStatus = atestadoStatus.Value;
        if (dataConsulta.HasValue) DataConsulta = dataConsulta.Value;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void SetSenha(SenhaAgendamento? senha)
    {
        SenhaAgendamento = senha;
        AtualizadoEm = DateTime.UtcNow;
    }
}
public class SenhaAgendamento
{
    public string Codigo { get; set; } = string.Empty;

    public DateOnly DataPedido { get; set; }

    public DateOnly? DataLiberacao { get; set; }

    public DateOnly? Validade { get; set; }
}
    public enum StatusAgendamento
    {
        [Display(Name = "Sem senha")]
        SemSenha = 0,
        [Display(Name = "Senha pendente")]
        SenhaPendente = 1,

        [Display(Name = "Senha Aprovada")]
        SenhaAprovada = 2,

        [Display(Name = "Agendamento Efetuado")]
        AgendamentoEfetuado = 3,

        [Display(Name = "Remarcada")]
        AgendamentoRemarcado = 4,
        [Display(Name = "Concluida")]
        ProcedimentoConcluido = 5,

        [Display(Name = "Cancelada")]
        Cancelada = 6
    }
    public enum StatusAtestado
    {
        [Display(Name = "Não realizado")]
        NaoRealizado = 0,
        [Display(Name = "Realizado")]
        Realizado = 1,

        [Display(Name = "Pendente")]
        Pendente = 2,

    }

    public enum StatusInstrucoes
    {
        [Display(Name = "Sem solicitação")]
        SemSolicitação = 0,
        [Display(Name = "Em analise")]
        EmAnalise = 1,

        [Display(Name = "Negado")]
        Negado = 2,
        [Display(Name = "Concluído")]
        Concluido = 3,

    }