using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DocAPI.Core.Models;

namespace DocAPI.Core.Models;
public class Agendamento
{
    [Key]
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string ID { get; set; } = string.Empty;
    public Agendamento() {}
    
    // public Agendamento(Paciente paciente)
    // { 
    //     Nome = paciente.Nome; 
    //     PacienteID = paciente.ID;
    //     ID = Guid.NewGuid().ToString();
    // }
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string PacienteID { get; set; } = string.Empty;
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string Nome { get; set; } = string.Empty;
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string Aviso { get; set; } = string.Empty;
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public DateOnly Data { get; set; } = DateOnly.MinValue;
    public TimeOnly Horario { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string Procedimento { get; set; } = string.Empty;
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string Local { get; set; } = string.Empty;
    public string Sala { get; set; } =string.Empty;
    public Senha? SenhaAgendamento { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public StatusAgendamento  Status { get; set; }
    public string StatusInstrucoes { get; set; }
    public string StatusAtestado { get; set; }
    public DateOnly DataConsulta { get; set; } = DateOnly.MinValue;

    public class Senha
    {
        public string Codigo { get; set; } = string.Empty;
        public DateOnly DataPedido { get; set; }
        public DateOnly? DataLibetracao { get; set; }
        public DateOnly? Validade { get; set; }

    }
    public enum StatusAgendamento
    {
        [Display(Name = "Sem senha")]//nao tem
        SemSenha = 1,
        [Display(Name = "Senha pendente")]//nao tem
        SenhaPendente = 2,

        [Display(Name = "Senha Aprovada")]
        SenhaAprovada = 3,

        [Display(Name = "Agendamento Efetuado")]
        AgendamentoEfetuado = 4,

        [Display(Name = "Remarcada")]
        AgendamentoRemarcado = 5,
        [Display(Name = "Concluida")]
        ProcedimentoConcluido = 6,

        [Display(Name = "Cancelada")]
        Cancelada = 7
    }

    
}