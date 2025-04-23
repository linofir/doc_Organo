using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DocAPI.Models;

namespace DocAPI.Models;
public class Agendamento
{
    public string ID { get; set; }
    
    public Agendamento(Paciente paciente)
    { 
        Nome = paciente.Nome; 
        ID = Guid.NewGuid().ToString();
    }
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string? Nome { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public DateTime? Data { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string? Cirurgia { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string? Local { get; set; }
    
}