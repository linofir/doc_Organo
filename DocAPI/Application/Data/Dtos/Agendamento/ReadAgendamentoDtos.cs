using System;
using DocAPI.Core.Entities;

namespace DocAPI.Data.Dtos.AgendamentoDtos
{
    public class ReadAgendamentoDto
    {
        public Guid ID { get; set; }

        public Guid AtendimentoId { get; set; }

        public Guid InternacaoId { get; set; }

        public Guid? PacienteId { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string? Aviso { get; set; }

        public DateOnly Data { get; set; }

        public TimeOnly Horario { get; set; }

        public string Local { get; set; } = string.Empty;

        public string? Sala { get; set; }

        public SenhaAgendamento? SenhaAgendamento { get; set; }

        public StatusAgendamento Status { get; set; }

        public StatusInstrucoes InstrucaoStatus { get; set; }

        public StatusAtestado AtestadoStatus { get; set; }

        public DateOnly DataConsulta { get; set; }

        public DateTime CriadoEm { get; set; }

        public DateTime? AtualizadoEm { get; set; }
    }
}