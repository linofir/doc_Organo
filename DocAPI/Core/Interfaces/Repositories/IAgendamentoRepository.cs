using System.Collections.Generic;
using System.Threading.Tasks;
using DocAPI.Core.Entities;

namespace DocAPI.Interfaces.Repositories;
public interface IAgendamentoRepository
{
    Task<IEnumerable<Agendamento>> GetAllAsync(int skip = 0, int take = 10);
    Task<Agendamento?> GetByIdAsync(string id);
    Task<List<Agendamento>> GetByNameAsync(string name);
    Task<List<Agendamento>> GetByPacienteIdAsync(string pacienteId);
    Task CreateAsync(Agendamento novoAgendamento);
    Task UpdateAsync(Agendamento agendamento, string id);
    Task DeleteAsync(string id);
}
