using System;
using DocAPI.Core.Entities;

namespace DocAPI.Data.Dtos.AgendamentoDtos
{
    public class UpdateAgendamentoDto
    {
        public string? Nome { get; set; }

        public string? Aviso { get; set; }

        public DateOnly? Data { get; set; }

        public TimeOnly? Horario { get; set; }

        public string? Local { get; set; }

        public string? Sala { get; set; }

        public SenhaAgendamento? SenhaAgendamento { get; set; }

        public StatusAgendamento? Status { get; set; }

        public StatusInstrucoes? InstrucaoStatus { get; set; }

        public StatusAtestado? AtestadoStatus { get; set; }

        public DateOnly? DataConsulta { get; set; }
    }
}