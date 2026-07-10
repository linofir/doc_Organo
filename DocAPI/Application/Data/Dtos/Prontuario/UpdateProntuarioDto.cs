namespace DocAPI.Data.Dtos.ProntuarioDtos;

/// <summary>
/// Correction-safe fields only — per D-01.
/// Evolution-only properties (Tipo, DataConsulta, AGO, collections, Internacao, etc.)
/// are structurally absent from this DTO.
/// </summary>
public class UpdateProntuarioDto
{
    /// <summary>Administrative / non-clinical notes; typo fixes.</summary>
    public string? InformacoesExtras { get; set; }

    /// <summary>Demographic snapshot corrections — Profissao, Religiao, AtividadeFisica only.
    /// Identity fields (NomePaciente, Cpf, Idade) and clinical narrative (QD)
    /// are immutable via correction.</summary>
    public string? Profissao { get; set; }
    public string? Religiao { get; set; }
    public string? AtividadeFisica { get; set; }
}