using System.Collections.Generic;
using System.Threading.Tasks;
using DocAPI.Core.Models;

namespace DocAPI.Core.Repositories;
public interface IPacienteRepository
{
    Task<IEnumerable<Paciente>> GetAllAsync(int skip = 0, int take = 10);
    Task<Paciente?> GetByIdAsync(string id);
    Task CreateAsync(Paciente novoPaciente);
    Task UpdateAsync(Paciente paciente, string id);
    Task DeleteAsync(string id);
}
