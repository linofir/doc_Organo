using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.ConstrainedExecution;
using System.Text.Json.Serialization;

namespace DocAPI.Core.Models;

public class Atendimento
{
    public Atendimento(){}
    // Propriedades gerais
    public string ID { get; set; }
    public string PacienteId { get; set; }
    public string NomePaciente { get; set; }
    public string ProntuarioId { get; set; }
    public string AgendamentoId { get; set; }
    public string EtapaAtualAtendimento { get; set; } // Ex: "Consulta", "Pré Procedimento"
    // public DateTime UltimaAtualizacao { get; set; }
    public string MensagemParaMedico { get; set; } // Ex: "Aguardando confirmação de exames"

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
    public List<CDStatus> CDsGeradas { get; set; } // CDs geradas na consulta
    public string StatusGeral { get; set; }
}

// Sub-DTO para a etapa de Pré Procedimento
public class PreProcedimentoEtapaStatus
{
    // CDs que precisam de progresso/confirmação
    public string StatusTermoCirurgico { get; set; } 
    public string StatusAnestesista { get; set; }
    public string StatusInstrumentadora { get; set; }
    public string StatusProcedimento { get; set; }
    public DateOnly? DataConsultaConcluida { get; set; }
    public string StatusExames { get; set; }
    public string StatusGeral { get; set; }
}
public class ProcedimentoEtapaStatus
{
    public string StatusAtestado { get; set; } 
    public string StatusGeral { get; set; }
}
public class PosProcedimentoEtapaStatus
{
    public string StatusConsultaPosOp { get; set; } 
    public DateOnly AgendamentoPosOp { get; set; }
    public string prontuarioPosOpId { get; set; } 
    public string StatusRecomendacoesMedicas { get; set; }
    public string StatusSeguimento { get; set; }
    public DateOnly PrevisaoSeguimento { get; set; }
    public DateOnly AlarmeSegimento { get; set; }
    public string StatusGeral { get; set; }
}

// DTO para o status de cada CD (Certificado/Documento)
public class CDStatus
{
    public string Descricao { get; set; }
    public string Status { get; set; } // Ex: "Pendente", "Confirmado", "Cancelado"
}