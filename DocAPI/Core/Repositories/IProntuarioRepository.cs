using System.Collections.Generic;
using System.Threading.Tasks;
using DocAPI.Core.Models;

namespace DocAPI.Core.Repositories;
public interface IProntuarioRepository
{
    Task<IEnumerable<Prontuario>> GetAllAsync(int skip = 0, int take = 10);
    Task<Prontuario?> GetByIdAsync(string id);
    Task CreateAsync(Prontuario novoProntuario);
    Task UpdateAsync(Prontuario prontuario, string id);
    Task DeleteAsync(string id);
}
