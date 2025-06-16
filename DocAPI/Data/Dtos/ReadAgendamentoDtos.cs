using System;
using System.ComponentModel.DataAnnotations;
using static DocAPI.Core.Models.Agendamento;

public class ReadAgendamentoDto
    {
        public string Nome { get; set; } = string.Empty;
        public string? Aviso { get; set; }  
        public DateOnly? Data { get; set; }
        public TimeOnly Horario { get; set; }
        public string Procedimento { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;
        public string? Sala { get; set; }
        public Senha? SenhaAgendamento { get; set; }
        public StatusAgendamento?  Status { get; set; }

    }

