using AutoMapper;
using DocAPI.Core.Entities;
using DocAPI.Data.Dtos.AgendamentoDtos;

namespace DocAPI.Profiles
{
    public class AgendamentoProfile : Profile
    {
        public AgendamentoProfile()
        {
            /* ---------- Agendamento ---------- */
            //Criação do ID e atrubuir os dados de paciente serão feitas no serviço 
            CreateMap<CreateAgendamentoDto, Agendamento>();
            CreateMap<Agendamento, UpdateAgendamentoDto>();
            CreateMap<UpdateAgendamentoDto, Agendamento>()
                // garante que ID e Nome (derivado do paciente) não sejam sobrescritos no update
                .ForMember(dest => dest.ID,   opt => opt.Ignore());
                // .ForMember(dest => dest.PacienteID,   opt => opt.Ignore())
                // .ForMember(dest => dest.Nome, opt => opt.Ignore());
            //Definir se preciso exibir os ID e PacienteID
            CreateMap<Agendamento, ReadAgendamentoDto>();
        }
    }
}
