using DocAPI.Core.Entities;
using DocAPI.Core.Interfaces.Repositories;
using DocAPI.Infrastructure.SqlDb.Context;
using Microsoft.EntityFrameworkCore;

namespace DocAPI.Infrastructure.Repositories;

public class PacienteRepository : IPacienteRepository
{
    private readonly DocDbContext _context;

    public PacienteRepository(DocDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Paciente>> GetAllAsync(int skip = 0, int take = 10)
    {
        return await _context.Pacientes
            .OrderBy(p => p.Nome)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<Paciente?> GetByIdAsync(Guid id)
    {
        return await _context.Pacientes
            .FirstOrDefaultAsync(p => p.ID == id);
    }

    public async Task<List<Paciente>> GetPacienteByCpfAsync(string cpf)
    {
        var cpfLimpo = cpf.Trim();
        return await _context.Pacientes
            .Where(p => p.CPF == cpfLimpo)
            .ToListAsync();
    }

    public async Task<List<Paciente>> GetPacienteByNomeAsync(string nome)
    {
        var nomeLimpo = nome.Trim();
        return await _context.Pacientes
            .Where(p => p.Nome.Contains(nomeLimpo))
            .ToListAsync();
    }

    public async Task CreateAsync(Paciente paciente)
    {
        paciente.AplicarCriacao();
        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Paciente paciente, Guid id)
    {
        var existing = await _context.Pacientes.FirstOrDefaultAsync(p => p.ID == id);
        if (existing == null)
            throw new KeyNotFoundException($"Paciente com ID '{id}' não encontrado.");

        existing.AplicarAtualizacao(
            paciente.Nome,
            paciente.Nascimento,
            paciente.CPF,
            paciente.RG,
            paciente.Email,
            paciente.Telefone,
            paciente.Plano,
            paciente.Carteira,
            paciente.Endereco);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await _context.Pacientes.FirstOrDefaultAsync(p => p.ID == id);
        if (existing == null)
            throw new KeyNotFoundException($"Paciente com ID '{id}' não encontrado.");

        existing.MarcarComoExcluido();
        await _context.SaveChangesAsync();
    }
}
