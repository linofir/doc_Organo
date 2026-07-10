using AutoMapper;
using DocAPI.Core.Entities;
using DocAPI.Data.Dtos.ProntuarioDtos;

namespace DocAPI.Profiles;

/// <summary>
/// Read-only projection profile for Prontuario.
/// Create / update / evolution write paths use domain factories and explicit mapping — no AutoMapper.
/// </summary>
public class ProntuarioProfile : Profile
{
    public ProntuarioProfile()
    {
        // ── Nested entity → DTO maps ──────────────────────────
        CreateMap<DescricaoBasica, DescricaoBasicaDto>();
        CreateMap<AGO, AGODto>();
        CreateMap<Antecedentes, AntecedentesDto>();
        CreateMap<AntecedentesFamiliares, AntecedentesFamiliaresDto>();
        CreateMap<PosOp, PosOpDto>();
        CreateMap<ProntuarioAcaoCD, AcoesCDDto>();
        CreateMap<Exame, ExameDto>();
        CreateMap<Internacao, SolicitacaoInternacaoDto>();
        CreateMap<ProcedimentoInternacao, ProcedimentoInternacaoDto>();

        // ── Prontuario → ReadProntuarioDto ────────────────────
        CreateMap<Prontuario, ReadProntuarioDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.ID))
            .ForMember(d => d.PacienteId, o => o.MapFrom(s => s.PacienteId))
            .ForMember(d => d.AtendimentoId, o => o.MapFrom(s => s.AtendimentoId))
            .ForMember(d => d.Versao, o => o.MapFrom(s => s.Versao))
            .ForMember(d => d.ProntuarioAnteriorId, o => o.MapFrom(s => s.ProntuarioAnteriorId))
            .ForMember(d => d.CriadoEm, o => o.MapFrom(s => s.CriadoEm))
            .ForMember(d => d.AtualizadoEm, o => o.MapFrom(s => s.AtualizadoEm))
            .ForMember(d => d.Deletado, o => o.MapFrom(s => s.Deletado))
            .ForMember(d => d.DataConsulta, o => o.MapFrom(s => s.DataConsulta))
            .ForMember(d => d.Tipo, o => o.MapFrom(s => s.Tipo))
            .ForMember(d => d.InformacoesExtras, o => o.MapFrom(s => s.InformacoesExtras))
            .ForMember(d => d.DescricaoBasica, o => o.MapFrom(s => s.DescricaoBasica))
            .ForMember(d => d.AGO, o => o.MapFrom(s => s.AGO))
            .ForMember(d => d.Antecedentes, o => o.MapFrom(s => s.Antecedentes))
            .ForMember(d => d.AntecedentesFamiliares, o => o.MapFrom(s => s.AntecedentesFamiliares))
            .ForMember(d => d.PosOperatorio, o => o.MapFrom(s => s.PosOperatorio))
            .ForMember(d => d.CD, o => o.MapFrom(s => s.AcoesCD))
            .ForMember(d => d.Exames, o => o.MapFrom(s => s.Exames))
            .ForMember(d => d.SolicitacaoInternacao, o => o.MapFrom(s => s.Internacao));
    }
}
