using System;
using System.Collections.Generic;

namespace DocAPI.Data.Dtos.ProntuarioDtos;

public class CreateProntuarioDto
{
    /// <summary>Root FK — Patient who owns this clinical record.</summary>
    public Guid PacienteId { get; set; }

    /// <summary>Root FK — Care journey this version belongs to.</summary>
    public Guid AtendimentoId { get; set; }

    public DateOnly DataConsulta { get; set; }

    /// <summary>Clinical classification (int per canonical type).</summary>
    public int Tipo { get; set; }

    public string? InformacoesExtras { get; set; }

    // ── Clinical sections (dedicated DTOs — no domain entity types) ──

    public DescricaoBasicaDto? DescricaoBasica { get; set; }
    public AGODto? AGO { get; set; }
    public AntecedentesDto? Antecedentes { get; set; }
    public AntecedentesFamiliaresDto? AntecedentesFamiliares { get; set; }
    public PosOpDto? PosOperatorio { get; set; }

    public List<AcoesCDDto>? CD { get; set; }
    public List<ExameDto>? Exames { get; set; }
    public SolicitacaoInternacaoDto? SolicitacaoInternacao { get; set; }
}

public class CreateVersaoProntuarioDto
{
    public DateOnly DataConsulta { get; set; }
    public int Tipo { get; set; }
    public string? InformacoesExtras { get; set; }

    public DescricaoBasicaDto? DescricaoBasica { get; set; }
    public AGODto? AGO { get; set; }
    public AntecedentesDto? Antecedentes { get; set; }
    public AntecedentesFamiliaresDto? AntecedentesFamiliares { get; set; }
    public PosOpDto? PosOperatorio { get; set; }

    public List<AcoesCDDto>? CD { get; set; }
    public List<ExameDto>? Exames { get; set; }
    public SolicitacaoInternacaoDto? SolicitacaoInternacao { get; set; }
}

// ── Nested DTOs ──────────────────────────────────────────

public class DescricaoBasicaDto
{
    public string NomePaciente { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public int Idade { get; set; }
    public string Profissao { get; set; } = string.Empty;
    public string Religiao { get; set; } = string.Empty;
    public string QD { get; set; } = string.Empty;
    public string? AtividadeFisica { get; set; }
}

public class AGODto
{
    public string Menarca { get; set; } = string.Empty;
    public string DUM { get; set; } = string.Empty;
    public string Paridade { get; set; } = string.Empty;
    public string DesejoGestacao { get; set; } = string.Empty;
    public Core.Entities.StatusVacinaHPV VacinaHPV { get; set; }
    public string CCO { get; set; } = string.Empty;
    public string MAC_TRH { get; set; } = string.Empty;
    public string Intercorrencias { get; set; } = string.Empty;
    public string Amamentacao { get; set; } = string.Empty;
    public string VidaSexual { get; set; } = string.Empty;
    public string Relacionamento { get; set; } = string.Empty;
    public string Parceiros { get; set; } = string.Empty;
    public string Coitarca { get; set; } = string.Empty;
    public string IST { get; set; } = string.Empty;
}

public class AntecedentesDto
{
    public string Comorbidades { get; set; } = string.Empty;
    public string Medicacao { get; set; } = string.Empty;
    public string Neoplasias { get; set; } = string.Empty;
    public string Cirurgias { get; set; } = string.Empty;
    public string Alergias { get; set; } = string.Empty;
    public string Vicios { get; set; } = string.Empty;
    public string HabitoIntestinal { get; set; } = string.Empty;
    public string Vacinas { get; set; } = string.Empty;
}

public class AntecedentesFamiliaresDto
{
    public string Neoplasias { get; set; } = string.Empty;
    public string Comorbidades { get; set; } = string.Empty;
}

public class PosOpDto
{
    public string? PeriodoSeguimento { get; set; }
    public string? Conclusao { get; set; }
    public string? ExameMacro { get; set; }
}

public class ExameDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Status { get; set; }
    public DateOnly? DataSolicitacao { get; set; }
    public DateOnly? DataResultado { get; set; }
}

public class AcoesCDDto
{
    public Core.Entities.AcoesCD Tipo { get; set; }
}

public class SolicitacaoInternacaoDto
{
    public List<ProcedimentoInternacaoDto>? Procedimentos { get; set; }
    public DateOnly Data { get; set; }
    public string IndicacaoClinica { get; set; } = string.Empty;
    public string Observacao { get; set; } = string.Empty;
    public string CIDCodigo { get; set; } = string.Empty;
    public string TempoDoenca { get; set; } = string.Empty;
    public int Diarias { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Regime { get; set; } = string.Empty;
    public string Carater { get; set; } = string.Empty;
    public bool UsaOPME { get; set; }
    public string Local { get; set; } = string.Empty;
    public string? Guia { get; set; }
}

public class ProcedimentoInternacaoDto
{
    public string CodigoProcedimento { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}