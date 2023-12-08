using AutoMapper;
using DocAPI.Data.Dtos;
using DocAPI.Models;

namespace DocAPI.Profiles;

public class ConsultorioProfile : Profile
{
    public  ConsultorioProfile()
    {
        CreateMap<CreateConsultorioDto, Consultorio>();
        CreateMap<Consultorio, ReadConsultorioDto>();
        // .ForMember( 
        //     consultorioDto => consultorioDto.Consultas,
        //     opt => opt.MapFrom( consultorio => consultorio.Consultas));
        CreateMap<UpdateConsultorioDto, Consultorio>();
        CreateMap<Consultorio, UpdateConsultorioDto>();

    }
}