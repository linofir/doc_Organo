using System.Collections.Generic;
using System.Threading.Tasks;
using DocAPI.Core.Models;

namespace DocAPI.Core.Repositories;
public interface IPacienteRepository
{
    Task<IEnumerable<Paciente>> GetAllAsync(int skip = 0, int take = 10);
    Task<Paciente?> GetByIdAsync(string id);
    Task<List<Paciente>> GetPacienteByCpfAsync(string cpf);
    Task<List<Paciente>> GetPacienteByNomeAsync(string nome);
    Task CreateAsync(Paciente novoPaciente);
    // Task<Stream> CreateReportByIdAsync(string pacienteId);
    // Task<Stream> CreateReportByCpfAsync(string pacienteCpf);
    Task UpdateAsync(Paciente paciente, string id);
    Task DeleteAsync(string id);
}
