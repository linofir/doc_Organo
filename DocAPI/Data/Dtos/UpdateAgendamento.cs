using System;
using System.ComponentModel.DataAnnotations;
public class UpdateAgendamentoDto
    {
        [Required(ErrorMessage = "Este campo é obrigatório")]
        public DateTime? Data { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Cirurgia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Local { get; set; } = string.Empty;
    }