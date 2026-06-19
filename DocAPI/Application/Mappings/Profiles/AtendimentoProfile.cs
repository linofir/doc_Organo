using DocAPI.Core.Entities;
using DocAPI.Data.Dtos.Atendimento;
using AutoMapper;

namespace DocAPI.Profiles;

public class AtendimentoProfile : Profile
{
    public AtendimentoProfile()
    {
        CreateMap<Atendimento, ReadAtendimentoDto>();
    }
}
