using DocAPI.Core.Entities;
using DocAPI.Core.Interfaces.Repositories;
using DocAPI.Services; //Planejando a implementação dos meus serviços como extração de dados do pdf


namespace DocAPI.Infrastructure.sqlDb.Repositories;

public class PacienteRepository : IPacienteRepository
{
    // public PacienteSheetsRepository()
    // {
        
    // }
    public async Task<IEnumerable<Paciente>> GetAllAsync(int skip = 0, int take = 10)
    {
        throw new NotImplementedException();
    }
    public async Task<Paciente?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
    public async Task<List<Paciente>> GetPacienteByCpfAsync(string cpf)
    {
        throw new NotImplementedException();
    }
    public async Task<List<Paciente>> GetPacienteByNomeAsync(string nome)
    {
        throw new NotImplementedException();
    }
    public async Task CreateAsync(Paciente paciente)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Paciente paciente, Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}