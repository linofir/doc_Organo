using System.ComponentModel.DataAnnotations;

namespace DocAPI.Core.Entities;

public class Agendamento
{
    protected Agendamento() { }

    public Agendamento(Guid internacaoId, Guid atendimentoId, DateOnly data, TimeOnly horario)
    {
        ID = Guid.NewGuid();
        InternacaoId = internacaoId;
        AtendimentoId = atendimentoId;
        Data = data;
        Horario = horario;
        CriadoEm = DateTime.UtcNow;
        Status = StatusAgendamento.SemSenha;
    }

    public Guid ID { get; private set; }

    public Guid InternacaoId { get; private set; }
    public Internacao Internacao { get; private set; } = null!;

    public Guid AtendimentoId { get; private set; }
    public Atendimento Atendimento { get; private set; } = null!;

    public string Nome { get; private set; } = string.Empty;

    public string Aviso { get; private set; } = string.Empty;

    public DateOnly Data { get; private set; }

    public TimeOnly Horario { get; private set; }

    public string Local { get; private set; } = string.Empty;

    public string Sala { get; private set; } = string.Empty;

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
}
public class SenhaAgendamento
{
    public string Codigo { get; private set; } = string.Empty;

    public DateOnly DataPedido { get; private set; }

    public DateOnly? DataLiberacao { get; private set; }

    public DateOnly? Validade { get; private set; }
}
    public enum StatusAgendamento
    {
        [Display(Name = "Sem senha")]//nao tem
        SemSenha = 0,
        [Display(Name = "Senha pendente")]//nao tem
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
        [Display(Name = "Não realizado")]//nao tem
        NaoRealizado = 0,
        [Display(Name = "Realizado")]//nao tem
        Realizado = 1,

        [Display(Name = "Pendente")]
        Pendente = 2,

    }

    public enum StatusInstrucoes
    {
        [Display(Name = "Sem solicitação")]//nao tem
        SemSolicitação = 0,
        [Display(Name = "Em analise")]//nao tem
        EmAnalise = 1,

        [Display(Name = "Negado")]
        Negado = 2,
        [Display(Name = "Concluído")]
        Concluido = 3,

    }

    
