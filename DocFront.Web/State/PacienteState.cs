using DocFront.Models;
using DocFront.Models.Dtos;
using DocFront.Mappers;
using DocFront.Services;
using DocFront.Utils;

public class PacienteState
{
    private readonly PacienteService _service;

    public PacienteState(PacienteService service)
    {
        _service = service;
    }
    public List<PacienteModel>? Lista { get; private set; }
    public PacienteModel? Selecionado { get; private set; }
    public List<PacienteListDto> Cards =>
    Lista is null ? new() : PacienteMapper.ToCard(Lista);

    public PacienteSearchType SearchType { get; private set; } = PacienteSearchType.Nenhum;
    public string? SearchValue { get; private set; }

    public bool IsLoading { get; private set; }
    public bool IsDirty { get; private set; }

    public event Action? OnChange;
    private void Notify() => OnChange?.Invoke();

    public async Task BuscarTodosAsync(bool force = false)
    {
        if (SearchType == PacienteSearchType.Todos && Lista != null && !force && !IsDirty)
        {
            Console.WriteLine("⚡ USANDO CACHE PACIENTE");
            return;
        }

        IsLoading = true;
        Notify();

        var response = await _service.GetAll();
        Console.WriteLine("🔥 CHAMANDO API PACIENTE");

        if (response.Success && response.Data != null)
        {
            Lista = response.Data;
            SearchType = PacienteSearchType.Todos;
            SearchValue = null;
            IsDirty = false;
        }else
        {
            Lista = new List<PacienteModel>(); 
        }

        IsLoading = false;
        Notify();
    }
    public async Task BuscarPorCpfAsync(string cpf, bool force = false)
    {
        if (SearchType == PacienteSearchType.Cpf && SearchValue == cpf && Lista != null && !force)
            return;

        IsLoading = true;
        Notify();

        var response = await _service.GetByCpf(cpf);

        if (response.Success && response.Data != null)
        {
            Lista = new List<PacienteModel> { response.Data! };
            SearchType = PacienteSearchType.Cpf;
            SearchValue = cpf;
            IsDirty = false;
        }else
        {
            Lista = new List<PacienteModel>(); 
        }
        

        IsLoading = false;
        Notify();
    }
    public async Task BuscarPorNomeAsync(string nome, bool force = false)
    {
        if (SearchType == PacienteSearchType.Nome && SearchValue == nome && Lista != null && !force)
            return;

        IsLoading = true;
        Notify();

        var response = await _service.GetByNome(nome);

        if (response.Success && response.Data != null)
        {
            Lista =  new List<PacienteModel> { response.Data! };
            SearchType = PacienteSearchType.Nome;
            SearchValue = nome;
            IsDirty = false;
        }else
        {
            Lista = new List<PacienteModel>(); 
        }

        IsLoading = false;
        Notify();
    }

    public async Task SelecionePorIdAsync(string id, bool force = false)
    {
        if (Selecionado != null && Selecionado.ID == id && !force)
        {
            Console.WriteLine("⚡ USANDO CACHE PACIENTE");
            return;
        }

        IsLoading = true;
        Notify();

        var response = await _service.GetById(id);
        Console.WriteLine("🔥 CHAMANDO API PACIENTE");
        if (response.Success)
            Selecionado = response.Data;
        else
            Selecionado = null;

        IsLoading = false;
        Notify();
    }
    public async Task<ApiResponse<bool>> EditarAsync(PacienteModel paciente)
    {
        IsLoading = true;
        Notify();

        var result = await _service.Update(paciente.ID, paciente);

        if (result.Success)
        {
            Selecionado = paciente;
            Invalidate(); // invalida listas
        }

        IsLoading = false;
        Notify();

        return result;
    }

    public async Task<ApiResponse<bool>> DeletarAsync(string id)
    {
        IsLoading = true;
        Notify();

        var result = await _service.Delete(id);

        if (result.Success)
        {
            Selecionado = null;
            Invalidate();
        }

        IsLoading = false;
        Notify();

        return result;
    }

    public async Task<ApiResponse<PacienteModel>> CriarAsync(PacienteModel model)
    {
        IsLoading = true;
        Notify();
        var dto = PacienteMapper.ToApi(model);
        var response = await _service.Create(dto);

        IsLoading = false;

        if (response.Success && response.Data != null)
            Invalidate();

        // if (!response.Success)
        // return ApiResponse<string>.Fail(response.Error!);

        Notify();
        Console.WriteLine(response.Data);
        
        return response;
    }

    public void Clear()
    {
        Lista = null;
        Selecionado = null;
        SearchType = PacienteSearchType.Nenhum;
        SearchValue = null;
        IsDirty = false;
        Notify();
    }
    public void Invalidate()
    {
        IsDirty = true;
    }
    public enum PacienteSearchType
    {
        Nenhum,
        Todos,
        Nome,
        Cpf
    }
}
