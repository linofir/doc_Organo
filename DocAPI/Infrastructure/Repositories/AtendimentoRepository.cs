using DocAPI.Core.Entities;
using DocAPI.Interfaces.Repositories;

namespace DocAPI.Infrastructure.Repositories;

/// <summary>
/// SQL implementation pending. Port rules from Legacy/_LegacySheetsDb/AtendimentoSheetsRepository.cs.
/// </summary>
public class AtendimentoRepository : IAtendimentoRepository
{
    private const string Message =
        "AtendimentoRepository SQL não implementado. Migração em andamento — ver docs/migration-sql.md.";

    public Task<IEnumerable<Atendimento>> GetAllAsync(int skip = 0, int take = 10) =>
        throw new NotImplementedException(Message);

    public Task<Atendimento?> GetByIdAsync(string id) =>
        throw new NotImplementedException(Message);

    public Task CreateAsync(Atendimento novoAtendimento) =>
        throw new NotImplementedException(Message);

    public Task UpdateAsync(Atendimento atendimento, string id) =>
        throw new NotImplementedException(Message);

    public Task DeleteAsync(string id) =>
        throw new NotImplementedException(Message);

    public Task<Stream> CreateReportByIdAsync(string pacienteId) =>
        throw new NotImplementedException(Message);

    public Task<Atendimento> CreateReportFollwUpByIdAsync(string pacienteId) =>
        throw new NotImplementedException(Message);
}
