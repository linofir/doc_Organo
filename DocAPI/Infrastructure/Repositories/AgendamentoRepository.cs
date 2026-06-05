using DocAPI.Core.Entities;
using DocAPI.Interfaces.Repositories;

namespace DocAPI.Infrastructure.Repositories;

/// <summary>
/// SQL implementation pending. See docs/migration-sql.md.
/// </summary>
public class AgendamentoRepository : IAgendamentoRepository
{
    private const string Message =
        "AgendamentoRepository SQL não implementado. Migração em andamento — ver docs/migration-sql.md.";

    public Task<IEnumerable<Agendamento>> GetAllAsync(int skip = 0, int take = 10) =>
        throw new NotImplementedException(Message);

    public Task<Agendamento?> GetByIdAsync(string id) =>
        throw new NotImplementedException(Message);

    public Task<List<Agendamento>> GetByNameAsync(string name) =>
        throw new NotImplementedException(Message);

    public Task<List<Agendamento>> GetByPacienteIdAsync(string pacienteId) =>
        throw new NotImplementedException(Message);

    public Task CreateAsync(Agendamento novoAgendamento) =>
        throw new NotImplementedException(Message);

    public Task UpdateAsync(Agendamento agendamento, string id) =>
        throw new NotImplementedException(Message);

    public Task DeleteAsync(string id) =>
        throw new NotImplementedException(Message);
}
