using System;
using System.ComponentModel.DataAnnotations;
using DocAPI.Core.Entities;

namespace DocAPI.Data.Dtos.AgendamentoDtos
{
    public class CreateAgendamentoDto
    {
        [Required(ErrorMessage = "O atendimento é obrigatório")]
        public Guid AtendimentoId { get; set; }

        [Required(ErrorMessage = "A internação é obrigatória")]
        public Guid InternacaoId { get; set; }

        public Guid? PacienteId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        public string? Aviso { get; set; }

        [Required(ErrorMessage = "A data é obrigatória")]
        public DateOnly Data { get; set; }

        public TimeOnly Horario { get; set; }

        public string? Local { get; set; }

        public string? Sala { get; set; }

        public SenhaAgendamento? SenhaAgendamento { get; set; }

        public StatusInstrucoes InstrucaoStatus { get; set; }

        public StatusAtestado AtestadoStatus { get; set; }

        public DateOnly DataConsulta { get; set; }
    }
}