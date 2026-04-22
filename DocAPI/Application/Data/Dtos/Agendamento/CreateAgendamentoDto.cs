using System;
using System.ComponentModel.DataAnnotations;
using  DocAPI.Core.Entities;

namespace DocAPI.Data.Dtos.AgendamentoDtos
{
    public class CreateAgendamentoDto
    {
        [Required(ErrorMessage = "O paciente é obrigatório")]
        public string PacienteId { get; set; } = string.Empty;
        [Required(ErrorMessage = "O paciente é obrigatório")]
        public string Nome { get; set; } = string.Empty;
        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string? Aviso { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        public DateOnly? Data { get; set; }
        public TimeOnly Horario { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string? Procedimento { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string? Local { get; set; }
        public string? Sala { get; set; }
        public SenhaAgendamento? SenhaAgendamento { get; set; }
        public StatusAgendamento?  Status { get; set; }
        public string StatusInstrucoes { get; set; } = string.Empty;
        public string StatusAtestado { get; set; } = string.Empty;
        public DateOnly DataConsulta { get; set; } = DateOnly.MinValue;
    }
}