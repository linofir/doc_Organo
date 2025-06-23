using System;
using System.ComponentModel.DataAnnotations;
using DocAPI.Core.Repositories;
using static DocAPI.Core.Models.Agendamento;

namespace DocAPI.Data.Dtos.Atendimento;
public class CreateAtendimentoDto
{
    public string EtapaAtendimento { get; set; } = string.Empty;
    public DateOnly DataConsulta { get; set; }


    
}
