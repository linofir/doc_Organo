using DocAPI.Core.Models;


namespace DocAPI.Core.Repositories;
public interface IAtendimentoRepository
{
    Task<IEnumerable<Atendimento>> GetAllAsync(int skip = 0, int take = 10);
    Task<Atendimento?> GetByIdAsync(string id);
    Task CreateAsync(Atendimento novoAtendimento);
    Task UpdateAsync(Atendimento atendimento, string id);
    Task DeleteAsync(string id);
    Task<Stream> CreateReportByIdAsync( string pacienteId );
    Task<Atendimento> CreateReportFollwUpByIdAsync( string pacienteId );
    
}