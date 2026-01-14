using DocFront.Utils;
using DocFront.Models;
using DocFront.Models.ViewModels;
using DocFront.Models.Dtos;
using DocFront.Mappers;

using System.Reflection.PortableExecutable;

namespace DocFront.Services;
//o ApiService já cria o HttpClient configurado para a API base.
public class ProntuarioService : ApiService
{
    public ProntuarioService(IHttpClientFactory factory) 
        : base(factory) {}
    public Task<ApiResponse<bool>> Create(ProntuarioViewModel model)
    { 
        var dto = ProntuarioMapper.ToApi(model);
        return PostAsync("prontuario", dto);
    }

    // public async Task<ApiResponse<List<PacienteListDto>>> GetAll()
    // {
    //     var response = await GetAsync<List<ProntuarioViewModel>>("prontuario");

    //     if (!response.Success || response.Data == null)
    //         return ApiResponse<List<PacienteListDto>>.Fail(response.Error ?? "Erro ao buscar prontuarios");

    //     var listDtos = response.Data.Select(p => ProntuarioMapper.ToViewModel(p)).ToList();

    //     return ApiResponse<List<PacienteListDto>>.Ok(listDtos);
    // }
    public async Task<ApiResponse<string>> GetTotal()
    {
        var response = await GetAsync<List<ProntuarioApiDto>>("prontuario");

        if (!response.Success || response.Data == null)
            return ApiResponse<string>.Fail(response.Error ?? "Erro ao buscar prontuarios");

        var totalProntuarios = response.Data.Count.ToString();

        return ApiResponse<string>.Ok(totalProntuarios);
    }
    public async Task<ApiResponse<ProntuarioViewModel>> GetById(string id)
    {
        var response = await GetAsync<ProntuarioApiDto>($"prontuario/{id}");

        if (!response.Success || response.Data is null)
            return ApiResponse<ProntuarioViewModel>.Fail(response.Error!);

        var api = response.Data;

        var prontuario = ProntuarioMapper.ToViewModel(api);

        return ApiResponse<ProntuarioViewModel>.Ok(prontuario!);
    }
    public async Task<ApiResponse<List<ProntuarioCardDto>>> GetByPaciente(string id)
    {
        Console.WriteLine("teste GetProntuariosByPaciente");
        var response = await GetAsync<List<ReadProntuarioDto>>($"prontuario/paciente/{id}");
        Console.WriteLine("teste depois da request");
        Console.WriteLine($"Success: {response.Success}");
        Console.WriteLine($"Data is null? {response.Data is null}");
        if (!response.Success)
        return ApiResponse<List<ProntuarioCardDto>>.Fail(response.Error!);

        if (response.Data == null)
        {
            Console.WriteLine("response.Data é NULL");
            return ApiResponse<List<ProntuarioCardDto>>.Ok(new());
        }

        Console.WriteLine($"Qtd recebida: {response.Data.Count}");

        var listProntuarios = ProntuarioMapper.ToProntuarioCard(response.Data);

        Console.WriteLine(listProntuarios?.Count.ToString());
        return ApiResponse<List<ProntuarioCardDto>>.Ok(listProntuarios!);
    }
   
    

    
    public Task<ApiResponse<ProntuarioViewModel>> GetByCpf(string cpf)
        => GetAsync<ProntuarioViewModel>($"prontuario/{cpf}");

    public Task<ApiResponse<bool>> Update(string id, ProntuarioViewModel model)
    {
        var dto = ProntuarioMapper.ToApi(model);
        return PutAsync($"prontuario/{id}", dto);
    }

    public Task<ApiResponse<bool>> Delete(string id)
        => DeleteAsync($"prontuario/{id}");
}
