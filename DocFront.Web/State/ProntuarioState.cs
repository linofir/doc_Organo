using DocFront.Models.ViewModels;
using DocFront.Models.Dtos;
using DocFront.Mappers;
using DocFront.Services;
using DocFront.Utils;
using DocFront.Models;

public class ProntuarioState
{
    private readonly ProntuarioService _service;

    public ProntuarioState(ProntuarioService service)
    {
        _service = service;
    }
    public List<ReadProntuarioDto>? Lista { get; private set; }
    public ProntuarioViewModel? Selecionado { get; private set; }
    public List<ProntuarioCardDto> Cards =>
    Lista is null ? new() : ProntuarioMapper.ToProntuarioCard(Lista)!;

    public ProntuarioSearchType SearchType { get; private set; } = ProntuarioSearchType.Nenhum;
    // public string? SearchValue { get; private set; }//quando implementar buscas específicas

    public bool IsLoadingProntuarios { get; private set; }
    public bool IsLoadingSelecionado { get; private set; }
    public bool IsDirty { get; private set; }
    public string? ErrorMessage { get; private set; }
    public bool Success { get; private set; }

    public event Action? OnChange;
    private void Notify() => OnChange?.Invoke();

    public async Task BuscarTodosPorPacienteAsync(string id,bool force = false)
    {
        if (SearchType == ProntuarioSearchType.PacienteID && Lista != null && !force && !IsDirty)
        {
            Console.WriteLine("⚡ USANDO CACHE Prontuarios de paciente");
            return;
        }

        IsLoadingProntuarios = true;
        ErrorMessage = null;
        Notify();

        var response = await _service.GetByPaciente(id);
        Console.WriteLine("🔥 CHAMANDO API para prontuarios de paciente");

        if (response.Success && response.Data != null)
        {
            Lista = response.Data;
            SearchType = ProntuarioSearchType.PacienteID;
            // SearchValue = null;
            IsDirty = false;
        }else
        {
            ErrorMessage = response.Error;
            Lista = new List<ReadProntuarioDto>(); 
        }

        IsLoadingProntuarios = false;
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
            Selecionado = ProntuarioMapper.ToViewModel(response.Data!);
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
        Selecionado = new ProntuarioViewModel
        {
            DescricaoBasica = new DescricaoBasicaViewModel
            {
                PacienteId = paciente.ID,
                NomePaciente = paciente.Nome,
                Cpf = paciente.CPF,
                Idade = paciente.Idade
            },
            AGO = new AGOViewModel(),
            Antecedentes = new AntecedentesViewModel(),
            AntecedentesFamiliares = new AntecedentesFamiliaresViewModel(),
            SolicitacaoInternacao = new InternacaoViewModel(),
            Exames = new List<ExameViewModel>(),
            PosOperatorio = new PosOpViewModel()
        };

        ErrorMessage = null;
        Notify();
    }

    public async Task<ApiResponse<bool>> CriarAsync(ProntuarioViewModel prontuario)
    {
        IsLoadingSelecionado = true;
        ErrorMessage = null;
        Notify();

        var dto = ProntuarioMapper.ToApi(prontuario);
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
    public async Task<ApiResponse<bool>> SalvarAsync(string id, ProntuarioViewModel prontuario)
    {
        IsLoadingSelecionado = true;
        Notify();
        ErrorMessage = null;
        var dto = ProntuarioMapper.ToApi(prontuario);
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
        IsLoadingProntuarios = true;
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
        SearchType = ProntuarioSearchType.Nenhum;
        ErrorMessage = null;
        // SearchValue = null;
        IsDirty = false;
        Notify();
    }
    public void Invalidate()
    {
        IsDirty = true;
    }
    public enum ProntuarioSearchType
    {
        Nenhum,
        PacienteID,
    }
}

