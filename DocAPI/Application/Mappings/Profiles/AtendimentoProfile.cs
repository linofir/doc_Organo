using DocAPI.Core.Entities;
using DocAPI.Data.Dtos.Atendimento;
using AutoMapper;

namespace DocAPI.Profiles;


public class AtendimentoProfile : Profile
{
    public AtendimentoProfile()
    {
        CreateMap<CreateAtendimentoDto , Atendimento>();
        CreateMap<UpdateAtendimentoDto, Atendimento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<Atendimento, UpdateAtendimentoDto>();
        CreateMap<Atendimento, ReadAtendimentoDto>();
       
    }
}