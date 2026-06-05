using DocAPI.Core.Entities;
using DocAPI.Interfaces.Repositories;

namespace DocAPI.Infrastructure.Repositories;

/// <summary>
/// SQL implementation pending. See docs/migration-sql.md (order: Paciente → Prontuario).
/// </summary>
public class ProntuarioRepository : IProntuarioRepository
{
    private const string Message =
        "ProntuarioRepository SQL não implementado. Migração em andamento — ver docs/migration-sql.md.";

    public Task<IEnumerable<Prontuario>> GetAllAsync(int skip = 0, int take = 10) =>
        throw new NotImplementedException(Message);

    public Task<Prontuario?> GetByIdAsync(string id) =>
        throw new NotImplementedException(Message);

    public Task<List<Prontuario>> GetProntuariosOfPacienteAsync(string pacienteId) =>
        throw new NotImplementedException(Message);

    public Task CreateAsync(Prontuario novoProntuario) =>
        throw new NotImplementedException(Message);

    public Task UpdateAsync(Prontuario prontuario, string id) =>
        throw new NotImplementedException(Message);

    public Task DeleteAsync(string id) =>
        throw new NotImplementedException(Message);

    public Task<Prontuario> CreateFromPdfAsync(string pacienteId, string pdfPath) =>
        throw new NotImplementedException(Message);
}
