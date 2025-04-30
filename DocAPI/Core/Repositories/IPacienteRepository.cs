using System.Collections.Generic;
using System.Threading.Tasks;
using DocAPI.Core.Models;

namespace DocAPI.Core.Repositories;
public interface IPacienteRepository
{
    /* Listar pacientes */
    Task<IEnumerable<Paciente>> GetAllAsync(int skip = 0, int take = 10);
    /* Listar pacientes por ID */
    Task<Paciente?> GetByIdAsync(string id);

    /* Criar um novo paciente */
    Task CreateAsync(Paciente novoPaciente);

    /* Atualizar um paciente existente */
    Task UpdateAsync(Paciente paciente, string id);

    // /* Remover um paciente pelo ID */
    Task DeleteAsync(string id);
}
