using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DocAPI.Models;

public class Consulta
{
    [Required(ErrorMessage = "A data é um valor necessário")]
    public DateTime Agendamento { get; set; }
    public string? Status { get; set; }
    // [Required(ErrorMessage = "É necessário indicar um consultório")]
    public string? ConsultorioID { get; set; }
    public virtual Consultorio? Local { get; set; }
    // [Required(ErrorMessage = "É necessário indicar um Paciente")]
    public string? PacienteID { get; set; }
    public virtual Paciente? Paciente { get; set; }

}