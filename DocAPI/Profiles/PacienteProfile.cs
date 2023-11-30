using DocAPI.Models;
using DocAPI.Data.Dtos;
using AutoMapper;

namespace DocAPI.Profiles;


public class PacienteProfile : Profile
{
    public PacienteProfile()
    {
        CreateMap<CreatePacienteDto , Paciente>();
        CreateMap<UpdatePacienteDto, Paciente>();
        CreateMap<Paciente, UpdatePacienteDto>();
        CreateMap<Paciente, ReadPacienteDto>();
    }
}