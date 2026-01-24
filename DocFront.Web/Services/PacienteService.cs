using DocFront.Utils;
using DocFront.Models;
using DocFront.Models.Dtos;
using DocFront.Mappers;

namespace DocFront.Services;
//o ApiService já cria o HttpClient configurado para a API base.
public class PacienteService : ApiService
{
    public PacienteService(IHttpClientFactory factory) 
        : base(factory) {}
    public async Task<ApiResponse<string>> Create(PacienteModel model)
    { 
        var dto = PacienteMapper.ToApi(model);
        var response = await PostAsync<PacienteApiDto, PacienteModel>(
                "paciente",
                dto
            );


        return ApiResponse<string>.Ok(response.Data!.ID);
        // return PostAsync("paciente", dto);
    }

    // public async Task<ApiResponse<List<PacienteListDto>>> GetAll()
    // {
    //     var response = await GetAsync<List<PacienteModel>>("paciente");

    //     if (!response.Success || response.Data == null)
    //         return ApiResponse<List<PacienteListDto>>.Fail(response.Error ?? "Erro ao buscar pacientes");
    //     var listDtos = PacienteMapper.ToCard(response.Data);

    //     return ApiResponse<List<PacienteListDto>>.Ok(listDtos);
    // }
    public async Task<ApiResponse<List<PacienteModel>>> GetAll()
    {
        var response = await GetAsync<List<PacienteModel>>("paciente");

        if (!response.Success || response.Data == null)
            return ApiResponse<List<PacienteModel>>.Fail(response.Error ?? "Erro ao buscar pacientes");
        // var listDtos = PacienteMapper.ToCard(response.Data);

        return ApiResponse<List<PacienteModel>>.Ok(response.Data);
    }
    public async Task<ApiResponse<PacienteModel>> GetById(string id)
    {
        var response = await GetAsync<PacienteApiDto>($"paciente/{id}");

        if (!response.Success || response.Data is null)
            return ApiResponse<PacienteModel>.Fail(response.Error!);

        var api = response.Data;
        var paciente = PacienteMapper.ToModel(id, api);

        return ApiResponse<PacienteModel>.Ok(paciente);
    }
    
    public async Task<ApiResponse<PacienteModel>> GetByCpf(string cpf)
    {
        var response = await GetAsync<PacienteApiDto>($"paciente/cpf/{cpf}");

        if (!response.Success || response.Data is null)
            return ApiResponse<PacienteModel>.Fail(response.Error!);

        var api = response.Data;
        var paciente = PacienteMapper.ToModel(api.ID, api);

        return ApiResponse<PacienteModel>.Ok(paciente);
    }
    public async Task<ApiResponse<PacienteModel>> GetByNome(string nome)
    {
        var response = await GetAsync<PacienteApiDto>($"paciente/nome/{nome}");

        if (!response.Success || response.Data is null)
            return ApiResponse<PacienteModel>.Fail(response.Error!);

        var api = response.Data;
        var paciente = PacienteMapper.ToModel(api.ID, api);

        return ApiResponse<PacienteModel>.Ok(paciente);
    }

    public Task<ApiResponse<bool>> Update(string id, PacienteModel model)
    {
        var dto = PacienteMapper.ToApi(model);
        return PutAsync($"paciente/{id}", dto);
    }

    public Task<ApiResponse<bool>> Delete(string id)
        => DeleteAsync($"paciente/{id}");
}
