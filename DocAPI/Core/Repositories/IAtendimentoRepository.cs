using DocAPI.Data.Dtos.Atendimento;

namespace DocAPI.Core.Repositories;
public interface IAtendimentoRepository
{
    Task<Stream> CreateReportByIdAsync(string pacienteId);
}