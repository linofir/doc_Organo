using System;
using System.ComponentModel.DataAnnotations;

public class ReadAgendamentoDto
    {
        public string ID   { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public DateTime? Data { get; set; }
        public string Cirurgia { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;

        public DateTime ConsultadoEm { get; set; } = DateTime.Now;
    }

