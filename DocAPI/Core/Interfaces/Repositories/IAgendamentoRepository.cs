using System.Collections.Generic;
using System.Threading.Tasks;
using DocAPI.Core.Entities;

namespace DocAPI.Interfaces.Repositories;
public interface IAgendamentoRepository
{
    Task<IEnumerable<Agendamento>> GetAllAsync(int skip = 0, int take = 10);
    Task<Agendamento?> GetByIdAsync(Guid id);
    Task<List<Agendamento>> GetByNameAsync(string name);
    Task<List<Agendamento>> GetByPacienteIdAsync(Guid pacienteId);
    Task CreateAsync(Agendamento novoAgendamento);
    Task UpdateAsync(Agendamento agendamento, Guid id);
    Task DeleteAsync(Guid id);
}
