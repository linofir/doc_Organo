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
                .ForPath(dest => dest.ID,   opt => opt.Ignore())               // chave não muda
                .ForPath(dest => dest.DescricaoBasica!.NomePaciente,
                         opt => opt.Ignore()) 
                .ForPath(dest => dest.DescricaoBasica!.PacienteId,
                         opt => opt.Ignore())                                 // vem do paciente
                .ForPath(dest => dest.DescricaoBasica!.Idade,
                        opt => opt.Ignore());
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
