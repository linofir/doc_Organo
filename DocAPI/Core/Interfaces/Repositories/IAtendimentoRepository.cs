using DocAPI.Core.Entities;

namespace DocAPI.Interfaces.Repositories;

public interface IAtendimentoRepository
{
    Task<IEnumerable<Atendimento>> GetAllAsync(int skip = 0, int take = 10);
    Task<Atendimento?> GetByIdAsync(Guid id);
    Task<IEnumerable<Atendimento>> GetByPacienteIdAsync(Guid pacienteId);
    Task CreateAsync(Atendimento novoAtendimento);
    Task UpdateAsync(Atendimento atendimento, Guid id);
    Task DeleteAsync(Guid id);
}
