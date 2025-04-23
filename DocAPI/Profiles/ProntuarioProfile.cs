using AutoMapper;
using DocAPI.Models;
using DocAPI.Data.Dtos.ProntuarioDtos;

namespace DocAPI.Profiles
{
    public class ProntuarioProfile : Profile
    {
        public ProntuarioProfile()
        {
            /* ---------- Prontuário ---------- */
            CreateMap<CreateProntuarioDto, Prontuario>();
            CreateMap<UpdateProntuarioDto, Prontuario>()
                .ForMember(dest => dest.ID,   opt => opt.Ignore())               // chave não muda
                .ForMember(dest => dest.DescricaoBasica!.NomePaciente,
                         opt => opt.Ignore())                                  // vem do paciente
                .ForMember(dest => dest.DescricaoBasica!.Idade,
                        opt => opt.Ignore());
            CreateMap<Prontuario, ReadProntuarioDto>();

            /* ---------- Descrição Básica ---------- */
            CreateMap<DescricaoBasicaDto, DescricaoBasica>();
            CreateMap<DescricaoBasica, DescricaoBasicaDto>();

            /* ---------- AGO ---------- */
            CreateMap<AgoDto, AGO>();
            CreateMap<AGO, AgoDto>();

            /* ---------- Antecedentes (AP) ---------- */
            CreateMap<AntecedentesDto, Antecedentes>();
            CreateMap<Antecedentes, AntecedentesDto>();

            /* ---------- Antecedentes Familiares (AF) ---------- */
            CreateMap<AntecedentesFamiliaresDto, AntecedentesFamiliares>();
            CreateMap<AntecedentesFamiliares, AntecedentesFamiliaresDto>();

            /* ---------- Exame ---------- */
            CreateMap<ExameDto, Exame>();
            CreateMap<Exame, ExameDto>();

            /* ---------- Internação ---------- */
            CreateMap<InternacaoDto, Internacao>();
            CreateMap<Internacao, InternacaoDto>();
        }
    }
}
