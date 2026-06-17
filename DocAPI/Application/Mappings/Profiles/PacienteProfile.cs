using DocAPI.Core.Entities;
using DocAPI.Data.Dtos;
using AutoMapper;

namespace DocAPI.Profiles;


public class PacienteProfile : Profile
{
    public PacienteProfile()
    {
        CreateMap<CreatePacienteDto, Paciente>()
            .ConstructUsing(dto => new Paciente(dto.Nome, dto.Nascimento, dto.CPF, dto.Email, dto.Telefone))
            .AfterMap((dto, dest) => dest.ComplementarCadastro(
                dto.RG,
                dto.Plano,
                dto.Carteira,
                dto.Endereco == null ? null : new Endereco
                {
                    Logradouro = dto.Endereco.Logradouro,
                    Numero = dto.Endereco.Numero,
                    Bairro = dto.Endereco.Bairro,
                    Cidade = dto.Endereco.Cidade,
                    UF = dto.Endereco.UF,
                    CEP = dto.Endereco.CEP
                }));
        CreateMap<UpdatePacienteDto, Paciente>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ConstructUsing(dto => new Paciente(dto.Nome, dto.Nascimento, dto.CPF, dto.Email, dto.Telefone))
            .AfterMap((dto, dest) => dest.ComplementarCadastro(
                dto.RG,
                dto.Plano,
                dto.Carteira,
                dto.Endereco == null ? null : new Endereco
                {
                    Logradouro = dto.Endereco.Logradouro,
                    Numero = dto.Endereco.Numero,
                    Bairro = dto.Endereco.Bairro,
                    Cidade = dto.Endereco.Cidade,
                    UF = dto.Endereco.UF,
                    CEP = dto.Endereco.CEP
                }));
        CreateMap<Paciente, UpdatePacienteDto>();
        CreateMap<Paciente, ReadPacienteDto>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID.ToString()));
        // .ForMember(
        //     pacienteDto => pacienteDto.Consultas,
        //     opt => opt.MapFrom( paciente => paciente.Consultas));
         // Mapeamento de Endereço
        CreateMap<CreateEnderecoDto, Endereco>();
        CreateMap<UpdateEnderecoDto, Endereco>();
        CreateMap<Endereco, UpdateEnderecoDto>();
        CreateMap<Endereco, ReadEnderecoDto>();
    }
}