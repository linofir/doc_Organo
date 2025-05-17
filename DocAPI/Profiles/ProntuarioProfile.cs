using AutoMapper;
using DocAPI.Core.Models;
using DocAPI.Data.Dtos.ProntuarioDtos;

namespace DocAPI.Profiles
{
    public class ProntuarioProfile : Profile
    {
        public ProntuarioProfile()
        {
            /* ---------- Prontuário ---------- */
            CreateMap<CreateProntuarioDto, Prontuario>();
            CreateMap<DescricaoBasicaDto, Core.Models.DescricaoBasica>();
            CreateMap<AGODto, Core.Models.AGO>();
            CreateMap<AntecedentesDto, Core.Models.Antecedentes>();
            CreateMap<AntecedentesFamiliaresDto, Core.Models.AntecedentesFamiliares>();
            CreateMap<ExameDto, Core.Models.Exame>();
            CreateMap<SolicitacaoInternacaoDto, Core.Models.Internacao>();
            CreateMap<UpdateProntuarioDto, Prontuario>()
                .ForMember(dest => dest.ID, opt => opt.Ignore())
                .ForMember(dest => dest.DescricaoBasica, opt => opt.MapFrom(src => src.DescricaoBasica))
                .ForMember(dest => dest.AGO, opt => opt.MapFrom(src => src.AGO))
                .ForMember(dest => dest.Antecedentes, opt => opt.MapFrom(src => src.Antecedentes))
                .ForMember(dest => dest.AntecedentesFamiliares, opt => opt.MapFrom(src => src.AntecedentesFamiliares))
                .ForMember(dest => dest.Exames, opt => opt.MapFrom(src => src.Exames))
                .ForMember(dest => dest.SolicitacaoInternacao, opt => opt.MapFrom(src => src.SolicitacaoInternacao));
            // CreateMap<UpdateProntuarioDto, Prontuario>()
            //     .ForPath(dest => dest.ID,   opt => opt.Ignore())               // chave não muda
            //     .ForPath(dest => dest.DescricaoBasica!.NomePaciente,
            //              opt => opt.Ignore()) 
            //     .ForPath(dest => dest.DescricaoBasica!.PacienteId,
            //              opt => opt.Ignore())                                 // vem do paciente
            //     .ForPath(dest => dest.DescricaoBasica!.Idade,
            //             opt => opt.Ignore())
            //     .ForPath(dest => dest.DescricaoBasica!.Cpf,
            //             opt => opt.Ignore());
            CreateMap<Prontuario, ReadProntuarioDto>();

            // /* ---------- Descrição Básica ---------- */
            // CreateMap<DescricaoBasicaDto, DescricaoBasica>();
            // CreateMap<DescricaoBasica, DescricaoBasicaDto>();

            // /* ---------- AGO ---------- */
            // CreateMap<AgoDto, AGO>();
            // CreateMap<AGO, AgoDto>();

            // /* ---------- Antecedentes (AP) ---------- */
            // CreateMap<AntecedentesDto, Antecedentes>();
            // CreateMap<Antecedentes, AntecedentesDto>();

            // /* ---------- Antecedentes Familiares (AF) ---------- */
            // CreateMap<AntecedentesFamiliaresDto, AntecedentesFamiliares>();
            // CreateMap<AntecedentesFamiliares, AntecedentesFamiliaresDto>();

            // /* ---------- Exame ---------- */
            // CreateMap<ExameDto, Exame>();
            // CreateMap<Exame, ExameDto>();

            // /* ---------- Internação ---------- */
            // CreateMap<InternacaoDto, Internacao>();
            // CreateMap<Internacao, InternacaoDto>();
        }
    }
}
