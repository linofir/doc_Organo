using DocAPI.Core.Entities;
using DocAPI.Infrastructure.SqlDb.Context;
using DocAPI.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DocAPI.Infrastructure.Repositories;

public class AgendamentoRepository : IAgendamentoRepository
{
    private readonly DocDbContext _context;

    public AgendamentoRepository(DocDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Agendamento>> GetAllAsync(int skip = 0, int take = 10)
    {
        take = Math.Clamp(take, 1, 100);
        return await _context.Agendamentos
            .OrderByDescending(a => a.Data)
            .ThenBy(a => a.Horario)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<Agendamento?> GetByIdAsync(Guid id)
    {
        return await _context.Agendamentos
            .FirstOrDefaultAsync(a => a.ID == id);
    }

    public async Task<List<Agendamento>> GetByNameAsync(string name)
    {
        return await _context.Agendamentos
            .Where(a => a.Nome.Contains(name))
            .ToListAsync();
    }

    public async Task<List<Agendamento>> GetByPacienteIdAsync(Guid pacienteId)
    {
        return await _context.Agendamentos
            .Where(a => a.PacienteID == pacienteId)
            .ToListAsync();
    }

    public async Task CreateAsync(Agendamento novoAgendamento)
    {
        await _context.Agendamentos.AddAsync(novoAgendamento);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Agendamento agendamento, Guid id)
    {
        _context.Agendamentos.Update(agendamento);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var agendamento = await _context.Agendamentos
            .FirstOrDefaultAsync(a => a.ID == id);
        if (agendamento == null) return;

        agendamento.SoftDelete();
        await _context.SaveChangesAsync();
    }
}