using AutoMapper;
using DocAPI.Data.Dtos;
using DocAPI.Models;

namespace DocAPI.Profiles;

public class ConsultaProfile : Profile
{
    public  ConsultaProfile()
    {
        CreateMap<CreateConsultaDto, Consulta>();
        CreateMap<Consulta, ReadConsultaDto>().ForMember(consultaDto => consultaDto.Consultorio, opt => opt.MapFrom(consulta => consulta.Local));
        CreateMap<UpdateConsultaDto, Consulta>();
        CreateMap<Consulta, UpdateConsultaDto>();

    }
}