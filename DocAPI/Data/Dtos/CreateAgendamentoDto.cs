using System;
using System.ComponentModel.DataAnnotations;

namespace DocAPI.Data.Dtos.AgendamentoDtos
{
    public class CreateAgendamentoDto
    {
        // Paciente já cadastrado (usado no serviço para popular Nome, se preferir)
        [Required(ErrorMessage = "O paciente é obrigatório")]
        public string PacienteId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public DateTime? Data { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Cirurgia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Local { get; set; } = string.Empty;
    }
}