using System;
using System.ComponentModel.DataAnnotations;
using  DocAPI.Core.Entities;

public class ReadAgendamentoDto
    {
        public string ID { get; set; } = string.Empty;
        public string PacienteID { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string? Aviso { get; set; }  
        public DateOnly? Data { get; set; }
        public TimeOnly Horario { get; set; }
        public string Procedimento { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;
        public string? Sala { get; set; }
        public SenhaAgendamento? SenhaAgendamento { get; set; }
        public StatusAgendamento?  Status { get; set; }
        public string StatusInstrucoes { get; set; } = string.Empty;
        public string StatusAtestado { get; set; } = string.Empty;
        public DateOnly DataConsulta { get; set; } = DateOnly.MinValue;

    }

