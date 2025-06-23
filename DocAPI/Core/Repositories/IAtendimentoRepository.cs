using DocAPI.Data.Dtos.Relatorio;

namespace DocAPI.Core.Repositories;
public interface IAtendimentoRepository
{
    Task<Stream> CreateReportByIdAsync(string pacienteId);
}