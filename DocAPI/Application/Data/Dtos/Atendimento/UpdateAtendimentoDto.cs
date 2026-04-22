using DocAPI.Core.Entities;

namespace DocAPI.Data.Dtos.Atendimento;
public class UpdateAtendimentoDto
{
    // Propriedades gerais
    // public string ID { get; set; } = string.Empty;
    public string PacienteId { get; set; } = string.Empty;
    public string NomePaciente { get; set; } = string.Empty;
    public List<string>? ProntuariosId { get; set; } 
    public List<string>? AgendamentosId { get; set; }
    public string EtapaAtualAtendimento { get; set; } = string.Empty; // Ex: "Consulta", "Pré Procedimento"
    // public DateTime UltimaAtualizacao { get; set; }
    public string MensagemParaMedico { get; set; } = string.Empty;// Ex: "Aguardando confirmação de exames"

    // Seções específicas para cada etapa, que podem ser nulas se a etapa não for relevante
    // public ConsultaEtapaStatus? EtapaConsulta { get; set; }
    // public PreProcedimentoEtapaStatus? EtapaPreProcedimento { get; set; }
    // public ProcedimentoEtapaStatus? EtapaProcedimento { get; set; }
    // public PosProcedimentoEtapaStatus? EtapaPosProcedimento { get; set; }

    
}

