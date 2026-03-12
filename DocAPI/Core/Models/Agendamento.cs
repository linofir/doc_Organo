using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DocAPI.Core.Models;

namespace DocAPI.Core.Models;
public class Agendamento
{
    // [Key]
    // [Required(ErrorMessage = "Este campo é obrigatório")]
    // public string ID { get; set; } = string.Empty;
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
    public string PacienteID { get; private set; } = string.Empty;
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string Nome { get; private set; } = string.Empty;
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string Aviso { get; private set; } = string.Empty;
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public DateOnly Data { get; private set; } = DateOnly.MinValue;
    public TimeOnly Horario { get; private set; }
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string Procedimento { get; private set; } = string.Empty;
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string Local { get; private set; } = string.Empty;
    public string Sala { get; private set; } =string.Empty;
    public Senha? SenhaAgendamento { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public StatusAgendamento  Status { get; set; }
    public StatusInstrucoes IntrucaoStatus { get; private set; }
    public StatusAtestado AtestadoStatus { get; private set; }
    public DateOnly DataConsulta { get; private set; } = DateOnly.MinValue;

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
    public enum StatusAtestado
    {
        [Display(Name = "Não realizado")]//nao tem
        NaoRealizado = 0,
        [Display(Name = "Realizado")]//nao tem
        Realizado = 1,

        [Display(Name = "Pendente")]
        Pendente = 2,

    }

    public enum StatusInstrucoes
    {
        [Display(Name = "Sem solicitação")]//nao tem
        SemSolicitação = 0,
        [Display(Name = "Em analise")]//nao tem
        EmAnalise = 1,

        [Display(Name = "Negado")]
        Negado = 2,
        [Display(Name = "Concluído")]
        Concluido = 3,

    }

    
}