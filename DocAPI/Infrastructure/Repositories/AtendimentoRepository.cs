using DocAPI.Core.Entities;
using DocAPI.Interfaces.Repositories;
using DocAPI.Infrastructure.SqlDb.Context;
using Microsoft.EntityFrameworkCore;

namespace DocAPI.Infrastructure.Repositories;

public class AtendimentoRepository : IAtendimentoRepository
{
    private readonly DocDbContext _context;

    public AtendimentoRepository(DocDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Atendimento>> GetAllAsync(int skip = 0, int take = 10)
    {
        return await _context.Atendimentos
            .OrderByDescending(a => a.CriadoEm)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<Atendimento?> GetByIdAsync(Guid id)
    {
        return await _context.Atendimentos
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Atendimento>> GetByPacienteIdAsync(Guid pacienteId)
    {
        return await _context.Atendimentos
            .Where(a => a.PacienteId == pacienteId)
            .OrderByDescending(a => a.CriadoEm)
            .ToListAsync();
    }

    public async Task CreateAsync(Atendimento novoAtendimento)
    {
        _context.Atendimentos.Add(novoAtendimento);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Atendimento atendimento, Guid id)
    {
        var existing = await _context.Atendimentos.FirstOrDefaultAsync(a => a.Id == id);
        if (existing == null)
            throw new KeyNotFoundException($"Atendimento com ID '{id}' não encontrado.");

        existing.AtualizarMensagemParaMedico(atendimento.MensagemParaMedico);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await _context.Atendimentos.FirstOrDefaultAsync(a => a.Id == id);
        if (existing == null)
            throw new KeyNotFoundException($"Atendimento com ID '{id}' não encontrado.");

        existing.MarcarComoExcluido();
        await _context.SaveChangesAsync();
    }
}
