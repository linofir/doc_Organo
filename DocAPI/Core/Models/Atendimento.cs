using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.ConstrainedExecution;
using System.Text.Json.Serialization;

namespace DocAPI.Core.Models;

public class Atendimento
{
    public Atendimento(){}
    // Propriedades gerais
    [Key]
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string ID { get; set; } = string.Empty;
    public string PacienteId { get; set; } = string.Empty;
    public string NomePaciente { get; set; } = string.Empty;
    public List<string>? ProntuariosId { get; set; }
    public List<string>? AgendamentosId { get; set; } 
    public string EtapaAtualAtendimento { get; set; } = string.Empty; // Ex: "Consulta", "Pré Procedimento"
    // public DateTime UltimaAtualizacao { get; set; }
    public string MensagemParaMedico { get; set; } = string.Empty;// Ex: "Aguardando confirmação de exames"

    // Seções específicas para cada etapa, que podem ser nulas se a etapa não for relevante
    public ConsultaEtapaStatus? EtapaConsulta { get; set; }
    public PreProcedimentoEtapaStatus? EtapaPreProcedimento { get; set; }
    public ProcedimentoEtapaStatus? EtapaProcedimento { get; set; }
    public PosProcedimentoEtapaStatus? EtapaPosProcedimento { get; set; }

}

// Sub-DTO para a etapa de Consulta
public class ConsultaEtapaStatus
{
    public bool CadastroConfirmado { get; set; }
    public bool ConsultaConcluida { get; set; }
    public DateOnly? DataConsultaConcluida { get; set; }
    public List<CDStatus>? CdPendente { get; set; } // CDs geradas na consulta
    public string StatusGeral { get; set; } = string.Empty;
}

// Sub-DTO para a etapa de Pré Procedimento
public class PreProcedimentoEtapaStatus
{
    // CDs que precisam de progresso/confirmação
    public string StatusTermoCirurgico { get; set; } = string.Empty;
    public string StatusEncaminhamento { get; set; } = string.Empty;
    public string StatusInstrumentadora { get; set; } = string.Empty;
    public string StatusExames { get; set; } = string.Empty;
    public string StatusGeral { get; set; } = string.Empty;
    public string StatusSenha { get; set; } = string.Empty;
    public string StatusAgendamento { get; set; } = string.Empty;
    public DateOnly? DataAgendamento { get; set; }
    public DateTime? UltimaAtualizacaoSenhas { get; set; }
    public List<string>? Procedimentos { get; set; }
}
public class ProcedimentoEtapaStatus
{
    public string StatusProcedimento { get; set; } = string.Empty;
    public string StatusAtestado { get; set; } = string.Empty;
    public string StatusInstrucoes { get; set; } = string.Empty;
    public string StatusGeral { get; set; } = string.Empty;
    public string StatusConsulta { get; set; } = string.Empty;
    public DateOnly dataConsultaPosOp { get; set; }
}
public class PosProcedimentoEtapaStatus
{
    public string StatusConsultaPosOp { get; set; } = string.Empty;
    public DateOnly AgendamentoPosOp { get; set; } 
    public string ProntuarioPosOpId { get; set; } = string.Empty;
    public string StatusRecomendacoesMedicas { get; set; } = string.Empty;
    public string StatusSeguimento { get; set; } = string.Empty;
    public DateOnly PrevisaoSeguimento { get; set; }
    public DateOnly AlarmeSegimento { get; set; }
    public string StatusGeral { get; set; } = string.Empty;
}

// DTO para o status de cada CD (Certificado/Documento)
public class CDStatus
{
    public string Descricao { get; set; } = string.Empty;
    public bool Pendente { get; set; } 
}
// public enum EtapaConsultaStatus
// {
//     [Display(Name = "Cadastro Pendente ")]
//     CadastroPendente = 0,

//     [Display(Name = "Pedido de exame")]
//     PedidoExame = 1,

//     [Display(Name = "Indicação de encaminhamentos")]
//     IndicacaoEncaminhamentos = 2,

//     [Display(Name = "Informativos de instrumentadora")]
//     InformativosInstrumentadora = 3,

//     [Display(Name = "Termo cirúrgico")]
//     TermoCirurgico = 4,

//     [Display(Name = "Pasta Informativa")]
//     PastaInformativa,
//     [Display(Name = "Sem Info")]
//     SemInformacao = 5
   
// }