using DocFront.Models.ViewModels;
using DocFront.Models.Dtos;

namespace DocFront.Mappers;
public static class AgendamentoMapper
{
    // Para endpints POST
    public static AgendamentoCreateApiDto ToApi(AgendamentoViewModel model)
    {
        return new AgendamentoCreateApiDto
        {
            PacienteId = model.PacienteID,
            Nome = model.Nome,
            Aviso = model.Aviso,
            Data = model.Data,
            Horario = model.Horario,
            Procedimento = model.Procedimento,
            Local = model.Local,
            Sala = model.Sala,
            SenhaAgendamento = MapSenhaAgendamento(model.SenhaAgendamento!),
            Status = model.Status,
            StatusInstrucoes = model.StatusInstrucoes,
            StatusAtestado = model.StatusAtestado,
            DataConsulta = model.DataConsulta,
            
        };
    }
    public static AgendamentoUpdateApiDto ToUpdateApi(this AgendamentoViewModel model)
    {
        return new AgendamentoUpdateApiDto
        {
            PacienteId = model.PacienteID!,
            Nome = model.Nome!,
            Aviso = model.Aviso!,
            Data = model.Data,
            Horario = model.Horario,
            Procedimento = model.Procedimento!,
            Local = model.Local!,
            Sala = model.Sala,
            SenhaAgendamento = MapSenhaAgendamento(model.SenhaAgendamento!),
            Status = model.Status,
            StatusInstrucoes = model.StatusInstrucoes,
            StatusAtestado = model.StatusAtestado
        };
    }
    private static SenhaDto MapSenhaAgendamento(SenhaViewModel vm)
    {
        return new SenhaDto
        {
            Codigo = vm.Codigo,
            DataPedido = vm.DataPedido,
            DataLibetracao = vm.DataLibetracao,
            Validade = vm.Validade,
        };
    }
    //Para endpoints Read
    public static AgendamentoViewModel? ToViewModel(AgendamentoReadApiDto dto)
    {
        if (dto == null) return null;
        return new AgendamentoViewModel
        {
            ID = dto.ID,
            PacienteID = dto.PacienteID,
            Nome = dto.Nome,
            Aviso = dto.Aviso,
            Data = dto.Data,  
            Horario = dto.Horario,
            Procedimento = dto.Procedimento,
            Local = dto.Local,
            Sala = dto.Sala,
            SenhaAgendamento = SenhaAgendamentoToViewModel(dto.SenhaAgendamento),
            Status = dto.Status,    
            StatusInstrucoes = dto.StatusInstrucoes,
            StatusAtestado = dto.StatusAtestado,
            DataConsulta = dto.DataConsulta,
        };
    }
    private static SenhaViewModel SenhaAgendamentoToViewModel(SenhaDto vm)
    {
        return new SenhaViewModel
        {
            Codigo = vm.Codigo,
            DataPedido = vm.DataPedido,
            DataLibetracao = vm.DataLibetracao,
            Validade = vm.Validade,
        };
    }

    // para o AgendamentoCard
    public static List<AgendamentoCardDto>? ToAgendamentoCard(List<AgendamentoReadApiDto> dtoList)
    {
        if (dtoList is null) return new List<AgendamentoCardDto>();

        var list = dtoList.Select(p => new AgendamentoCardDto
        {
            ID = p.ID,
            Data = p.Data,
            Status = p.Status,
        }).ToList();
        return list;
    }
}

