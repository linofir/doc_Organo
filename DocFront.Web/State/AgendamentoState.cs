using DocFront.Models.ViewModels;
using DocFront.Models.Enums;
using DocFront.Models.Dtos;
using DocFront.Mappers;
using DocFront.Services;
using DocFront.Utils;
using DocFront.Models;

public class AgendamentoState
{
    private readonly AgendamentoService _service;

    public AgendamentoState(AgendamentoService service)
    {
        _service = service;
    }
    public List<AgendamentoReadApiDto>? Lista { get; private set; }
    public AgendamentoViewModel? Selecionado { get; private set; }
    public List<AgendamentoCardDto> Cards =>
    Lista is null ? new() : AgendamentoMapper.ToAgendamentoCard(Lista)!;

    public AgendamentoSearchType SearchType { get; private set; } = AgendamentoSearchType.Nenhum;
    // public string? SearchValue { get; private set; }//quando implementar buscas específicas

    public bool IsLoadingAgendamentos { get; private set; }
    public bool IsLoadingSelecionado { get; private set; }
    public bool IsDirty { get; private set; }
    public string? ErrorMessage { get; private set; }
    public bool Success { get; private set; }

    public event Action? OnChange;
    private void Notify() => OnChange?.Invoke();

    public async Task BuscarTodosPorPacienteAsync(string id,bool force = false)
    {
        if (SearchType == AgendamentoSearchType.PacienteID && Lista != null && !force && !IsDirty)
        {
            Console.WriteLine("⚡ USANDO CACHE Agenmento de paciente");
            return;
        }

        IsLoadingAgendamentos = true;
        ErrorMessage = null;
        Notify();

        var response = await _service.GetByPaciente(id);
        Console.WriteLine("🔥 CHAMANDO API para Agenmento de paciente");

        if (response.Success && response.Data != null)
        {
            Lista = response.Data;
            SearchType = AgendamentoSearchType.PacienteID;
            // SearchValue = null;
            IsDirty = false;
        }else
        {
            ErrorMessage = response.Error;
            Lista = new List<AgendamentoReadApiDto>(); 
        }

        IsLoadingAgendamentos = false;
        Notify();
    }
    public async Task BuscarPorIdAsync(string id, bool force = false)
    {
        if (Selecionado != null && Selecionado.ID == id && !force)
        {
            Console.WriteLine("⚡ USANDO CACHE prontuario");
            return;
        }

        IsLoadingSelecionado = true;
        ErrorMessage = null;
        Notify();

        var response = await _service.GetById(id);
        
        Console.WriteLine("🔥 CHAMANDO API Prontuario Selecioando");
        if (response.Success)
            Selecionado =  AgendamentoMapper.ToViewModel(response.Data!);
        else
        {
            Selecionado = null;
            ErrorMessage = response.Error;
        }

        IsLoadingSelecionado = false;
        Notify();
    }
    public void PrepararNovo(PacienteModel paciente)
    {
        Selecionado = new AgendamentoViewModel
        {
           PacienteID = paciente!.ID,
            Nome = paciente!.Nome,
            Aviso = "",
            Data = DateOnly.MinValue,  
            Horario = TimeOnly.MinValue,
            Procedimento = "",
            Local = "",
            Sala = "",
            SenhaAgendamento = new SenhaViewModel(),
            Status = StatusAgendamento.SemSenha,//,  Preciso saber como atribuir valor padrão nessa propriedade   
            StatusInstrucoes = "",
            StatusAtestado = "",
            DataConsulta = DateOnly.MinValue,
        };

        ErrorMessage = null;
        Notify();
    }

    public async Task<ApiResponse<bool>> CriarAsync(AgendamentoViewModel prontuario)
    {
        IsLoadingSelecionado = true;
        ErrorMessage = null;
        Notify();

        var dto = AgendamentoMapper.ToApi(prontuario);
        var response = await _service.Create(dto);

        IsLoadingSelecionado = false;

        if (response.Success)
        {
            Success = true;
            Invalidate();
        }else
        {
            Success = false;
            ErrorMessage = response.Error;
        }

        Notify();
        return response;
    }
    public async Task<ApiResponse<bool>> SalvarAsync(string id, AgendamentoViewModel prontuario)
    {
        IsLoadingSelecionado = true;
        Notify();
        ErrorMessage = null;
        var dto = AgendamentoMapper.ToApi(prontuario);
        var result = await _service.Update(id, dto);

        if (result.Success)
        {
            Selecionado = prontuario;
            Invalidate(); // invalida listas
        }else
        {
            ErrorMessage = result.Error;
        }

        IsLoadingSelecionado = false;
        Notify();
        return result;
    }
    public async Task<ApiResponse<bool>> DeletarAsync(string id)
    {
        IsLoadingAgendamentos = true;
        Notify();

        var result = await _service.Delete(id);

        if (result.Success)
        {
            Selecionado = null;
            Invalidate();
        }

        IsLoadingSelecionado = false;
        Notify();

        return result;
    }
    public void Clear()
    {
        Lista = null;
        Selecionado = null;
        SearchType = AgendamentoSearchType.Nenhum;
        ErrorMessage = null;
        // SearchValue = null;
        IsDirty = false;
        Notify();
    }
    public void Invalidate()
    {
        IsDirty = true;
    }
    public enum AgendamentoSearchType
    {
        Nenhum,
        PacienteID,
    }
}

