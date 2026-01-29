using DocFront.Utils;
using DocFront.Models;
using DocFront.Models.ViewModels;
using DocFront.Models.Dtos;
using DocFront.Mappers;

using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace DocFront.Services;
//o ApiService já cria o HttpClient configurado para a API base.
public class AgendamentoService : ApiService
{

    public AgendamentoService(IHttpClientFactory factory) 
        : base(factory) {}
    public Task<ApiResponse<bool>> Create(AgendamentoCreateApiDto dto)
    { 
        // var dto = AgendamentoMapper.ToApi(model);
        return PostAsync("agendamento", dto);
    }
    public async Task<ApiResponse<string>> GetTotal()
    {
        var response = await GetAsync<List<ProntuarioApiDto>>("agendamento");

        if (!response.Success || response.Data == null)
            return ApiResponse<string>.Fail(response.Error ?? "Erro ao buscar agendamentos");

        var totalAgendamentos = response.Data.Count.ToString();

        return ApiResponse<string>.Ok(totalAgendamentos);
    }
    public async Task<ApiResponse<AgendamentoReadApiDto>> GetById(string id)
    {
        var response = await GetAsync<AgendamentoReadApiDto>($"agendamento/{id}");

        // if (!response.Success || response.Data is null)
        //     return ApiResponse<AgendamentoViewModel>.Fail(response.Error!);

        // var api = response.Data;

        // var agendamento = AgendamentoMapper.ToViewModel(api);

        return response;
    }
    public async Task<ApiResponse<List<AgendamentoReadApiDto>>> GetByPaciente(string id)
    {
        var response = await GetAsync<List<AgendamentoReadApiDto>>($"agendamento/by-pacientId/?pacienteId={id}");
      
        // if (!response.Success)
        // return ApiResponse<List<AgendamentoCardDto>>.Fail(response.Error!);

        // if (response.Data == null)
        // {
        //     Console.WriteLine("response.Data é NULL");
        //     return ApiResponse<List<AgendamentoCardDto>>.Ok(new());
        // }

        // var listAgendamentos = AgendamentoMapper.ToAgendamentoCard(response.Data);

        // Console.WriteLine(listAgendamentos?.Count.ToString());
        return response;
    }
    // public Task<ApiResponse<AgendamentoViewModel>> GetByCpf(string cpf)
    //     => GetAsync<AgendamentoViewModel>($"prontuario/{cpf}");

    public Task<ApiResponse<bool>> Update(string id, AgendamentoCreateApiDto dto)
    {
        // var dto = AgendamentoMapper.ToUpdateApi(model);
        // var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions
        //     {
        //         WriteIndented = true
        //     });

        // Console.WriteLine("JSON ENVIADO:");
        // Console.WriteLine(json);
        return PutAsync($"agendamento/{id}", dto);
    }

    public Task<ApiResponse<bool>> Delete(string id)
        => DeleteAsync($"agendamento/{id}");
}
