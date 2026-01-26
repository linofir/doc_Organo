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
    public Task<ApiResponse<bool>> Create(ProntuarioApiDto dto)
    { 
        // var dto = ProntuarioMapper.ToApi(model);
        return PostAsync("prontuario", dto);
    }
    public async Task<ApiResponse<string>> GetTotal()
    {
        var response = await GetAsync<List<ProntuarioApiDto>>("prontuario");

        if (!response.Success || response.Data == null)
            return ApiResponse<string>.Fail(response.Error ?? "Erro ao buscar prontuarios");

        var totalProntuarios = response.Data.Count.ToString();

        return ApiResponse<string>.Ok(totalProntuarios);
    }
    public async Task<ApiResponse<ReadProntuarioDto>> GetById(string id)
    {
        var response = await GetAsync<ReadProntuarioDto>($"prontuario/{id}");

        // if (!response.Success || response.Data is null)
        //     return ApiResponse<ProntuarioViewModel>.Fail(response.Error!);

        // var api = response.Data;

        // var prontuario = ProntuarioMapper.ToViewModel(api);

        return response;
    }
    // public async Task<ApiResponse<List<ProntuarioCardDto>>> GetByPaciente(string id)
    // {
    //     var response = await GetAsync<List<ReadProntuarioDto>>($"prontuario/paciente/{id}");
    //     if (!response.Success)
    //     return ApiResponse<List<ProntuarioCardDto>>.Fail(response.Error!);

    //     if (response.Data == null)
    //     {
    //         Console.WriteLine("response.Data é NULL");
    //         return ApiResponse<List<ProntuarioCardDto>>.Ok(new());
    //     }

    //     var listProntuarios = ProntuarioMapper.ToProntuarioCard(response.Data);

    //     return ApiResponse<List<ProntuarioCardDto>>.Ok(listProntuarios!);
    // }
    public async Task<ApiResponse<List<ReadProntuarioDto>>> GetByPaciente(string id)
    {
        var response = await GetAsync<List<ReadProntuarioDto>>($"prontuario/paciente/{id}");
        return response;
    }
    public Task<ApiResponse<ProntuarioViewModel>> GetByCpf(string cpf)
        => GetAsync<ProntuarioViewModel>($"prontuario/{cpf}");

    public Task<ApiResponse<bool>> Update(string id, ProntuarioApiDto dto)
    {
        // var dto = ProntuarioMapper.ToApi(model);
        return PutAsync($"prontuario/{id}", dto);
    }

    public Task<ApiResponse<bool>> Delete(string id)
        => DeleteAsync($"prontuario/{id}");
}
