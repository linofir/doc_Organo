using AutoMapper;
using DocAPI.Models;
using DocAPI.Data.Dtos.AgendamentoDtos;

namespace DocAPI.Profiles
{
    public class AgendamentoProfile : Profile
    {
        public AgendamentoProfile()
        {
            /* ---------- Agendamento ---------- */
            CreateMap<CreateAgendamentoDto, Agendamento>();

            CreateMap<UpdateAgendamentoDto, Agendamento>()
                // garante que ID e Nome (derivado do paciente) não sejam sobrescritos no update
                .ForMember(dest => dest.ID,   opt => opt.Ignore())
                .ForMember(dest => dest.Nome, opt => opt.Ignore());

            CreateMap<Agendamento, ReadAgendamentoDto>();
        }
    }
}
