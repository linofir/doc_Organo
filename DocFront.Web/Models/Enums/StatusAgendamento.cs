using System.ComponentModel.DataAnnotations;
namespace DocFront.Models.Enums;

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