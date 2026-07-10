using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocAPI.Core.Entities;
using DocAPI.Infrastructure.SqlDb.Context;
using DocAPI.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DocAPI.Infrastructure.Repositories;

public class ProntuarioRepository : IProntuarioRepository
{
    private readonly DocDbContext _context;

    public ProntuarioRepository(DocDbContext context)
    {
        _context = context;
    }

    // ── Reads ──────────────────────────────────────────────

    public async Task<Prontuario?> GetByIdAsync(Guid id)
    {
        return await QueryWithIncludes()
            .FirstOrDefaultAsync(p => p.ID == id);
    }

    public async Task<IEnumerable<Prontuario>> GetAllAsync(int skip = 0, int take = 10)
    {
        return await QueryWithIncludes()
            .OrderByDescending(p => p.Versao)
            .ThenByDescending(p => p.CriadoEm)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<Prontuario>> GetByPacienteIdAsync(Guid pacienteId)
    {
        return await QueryWithIncludes()
            .Where(p => p.PacienteId == pacienteId)
            .OrderByDescending(p => p.Versao)
            .ThenByDescending(p => p.CriadoEm)
            .ToListAsync();
    }

    public async Task<Prontuario?> GetLatestByPacienteIdAsync(Guid pacienteId)
    {
        return await QueryWithIncludes()
            .Where(p => p.PacienteId == pacienteId)
            .OrderByDescending(p => p.Versao)
            .ThenByDescending(p => p.CriadoEm)
            .FirstOrDefaultAsync();
    }

    public async Task<Prontuario?> GetLatestByAtendimentoIdAsync(Guid atendimentoId)
    {
        return await QueryWithIncludes()
            .Where(p => p.AtendimentoId == atendimentoId)
            .OrderByDescending(p => p.Versao)
            .ThenByDescending(p => p.CriadoEm)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetMaxVersaoForPacienteAsync(Guid pacienteId)
    {
        // Ignore global soft-delete filter so a deleted version doesn't cause duplicate Versao assignment,
        // but the max value across ALL rows (including soft-deleted) still correctly reflects occupied slots.
        return await _context.Prontuarios
            .IgnoreQueryFilters()
            .Where(p => p.PacienteId == pacienteId)
            .MaxAsync(p => (int?)p.Versao) ?? 0;
    }

    // ── Writes ─────────────────────────────────────────────

    public async Task AddAsync(Prontuario prontuario)
    {
        _context.Prontuarios.Add(prontuario);
        await _context.SaveChangesAsync();
    }

    /// <summary>In-place correction — scalar and owned VO updates only.
    /// Does NOT mutate child collections per D-01.</summary>
    public async Task UpdateAsync(Prontuario prontuario)
    {
        var existing = await _context.Prontuarios
            .FirstOrDefaultAsync(p => p.ID == prontuario.ID);

        if (existing == null)
            throw new KeyNotFoundException($"Prontuario '{prontuario.ID}' não encontrado.");

        // Correction-safe scalars
        existing.AplicarCorrecao(
            prontuario.InformacoesExtras,
            prontuario.DescricaoBasica.Profissao,
            prontuario.DescricaoBasica.Religiao,
            prontuario.DescricaoBasica.AtividadeFisica);

        await _context.SaveChangesAsync();
    }

    /// <summary>Soft-deletes a single version.
    /// Caller has already invoked MarcarComoExcluido on the aggregate.</summary>
    public async Task DeleteAsync(Prontuario prontuario)
    {
        var existing = await _context.Prontuarios
            .FirstOrDefaultAsync(p => p.ID == prontuario.ID);

        if (existing == null)
            throw new KeyNotFoundException($"Prontuario '{prontuario.ID}' não encontrado.");

        existing.MarcarComoExcluido();
        await _context.SaveChangesAsync();
    }

    // ── Helpers ────────────────────────────────────────────

    private IQueryable<Prontuario> QueryWithIncludes()
    {
        return _context.Prontuarios
            .Include(p => p.AcoesCD)
            .Include(p => p.Exames)
            .Include(p => p.Internacao)
            .ThenInclude(i => i!.Procedimentos);
    }
}