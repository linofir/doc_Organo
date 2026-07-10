using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocAPI.Core.Entities;

namespace DocAPI.Interfaces.Repositories;

/// <summary>
/// Persistence contract for the Prontuario aggregate.
/// All IDs are Guid; version lookup returns persistence facts only — business policy lives in the aggregate per ADR-006 and D-02.
/// </summary>
public interface IProntuarioRepository
{
    // ── Reads ──────────────────────────────────────────────

    Task<Prontuario?> GetByIdAsync(Guid id);

    Task<IEnumerable<Prontuario>> GetAllAsync(int skip = 0, int take = 10);

    /// <summary>All non-deleted versions for a patient, ordered by Versao DESC then CriadoEm DESC.</summary>
    Task<List<Prontuario>> GetByPacienteIdAsync(Guid pacienteId);

    /// <summary>Highest Versao among non-deleted for the patient, or null when none.</summary>
    Task<Prontuario?> GetLatestByPacienteIdAsync(Guid pacienteId);

    /// <summary>Highest Versao among non-deleted for the atendimento, or null when none.</summary>
    Task<Prontuario?> GetLatestByAtendimentoIdAsync(Guid atendimentoId);

    /// <summary>Persistence lookup only — returns max existing Versao for the patient, or 0 when none.
    /// Does NOT decide business version policy (see D-02).</summary>
    Task<int> GetMaxVersaoForPacienteAsync(Guid pacienteId);

    // ── Writes ─────────────────────────────────────────────

    /// <summary>Persists a new Prontuario graph (v1 create or evolution successor).</summary>
    Task AddAsync(Prontuario prontuario);

    /// <summary>Persists in-place correction updates. Does NOT touch child collections (D-01).</summary>
    Task UpdateAsync(Prontuario prontuario);

    /// <summary>Soft-deletes a single version. Caller is responsible for invoking MarcarComoExcluido first.</summary>
    Task DeleteAsync(Prontuario prontuario);
}