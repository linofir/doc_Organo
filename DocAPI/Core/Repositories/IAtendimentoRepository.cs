using DocAPI.Core.Models;


namespace DocAPI.Core.Repositories;
public interface IAtendimentoRepository
{
    Task<Stream> CreateReportByIdAsync( string pacienteId );
    Task<Atendimento> CreateReportFollwUpByIdAsync( string pacienteId );
}