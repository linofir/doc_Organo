using AutoMapper;
using DocAPI.Core.Entities;
using DocAPI.Data.Dtos.AgendamentoDtos;

namespace DocAPI.Profiles
{
    public class AgendamentoProfile : Profile
    {
        public AgendamentoProfile()
        {
            // Read-direction mapping only — write paths use manual aggregate construction.
            CreateMap<Agendamento, ReadAgendamentoDto>();
        }
    }
}