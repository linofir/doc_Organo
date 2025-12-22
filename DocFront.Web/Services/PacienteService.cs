using DocFront.Utils;
using DocFront.Models;
using DocFront.Models.Dtos;

namespace DocFront.Services;
//o ApiService já cria o HttpClient configurado para a API base.
public class PacienteService : ApiService
{
    public PacienteService(IHttpClientFactory factory) 
        : base(factory) {}
    public Task<ApiResponse<bool>> Create(PacienteModel model)
    { 
        var dto =new PacienteApiDto
        {
            Nome = model.Nome,
            CPF = model.CPF,
            Nascimento = model.Nascimento,
            Email = model.Email,
            Telefone = model.Telefone,
            Plano = model.Plano,
            Carteira = model.Carteira,
            RG = model.RG,

            Endereco = new EnderecoDto
            {
                Logradouro = model.Logradouro,
                Numero = model.Numero,
                Bairro = model.Bairro,
                Cidade = model.Cidade,
                UF = model.UF,
                CEP = model.CEP
            }
        };
        return PostAsync("paciente", dto);
    }

    public async Task<ApiResponse<List<PacienteListDto>>> GetAll()
    {
        var response = await GetAsync<List<PacienteModel>>("paciente");

        if (!response.Success || response.Data == null)
            return ApiResponse<List<PacienteListDto>>.Fail(response.Error ?? "Erro ao buscar pacientes");

        var listDtos = response.Data.Select(p => new PacienteListDto
        {
            Id = p.ID,
            Nome = p.Nome,
            CPF = p.CPF,
            Email = p.Email
        }).ToList();

        return ApiResponse<List<PacienteListDto>>.Ok(listDtos);
    }
    public async Task<ApiResponse<PacienteModel>> GetById(string id)
    {
        var response = await GetAsync<PacienteApiDto>($"paciente/{id}");

        if (!response.Success || response.Data is null)
            return ApiResponse<PacienteModel>.Fail(response.Error!);

        var api = response.Data;

        var paciente = new PacienteModel
        {
            ID = id,
            Nome = api.Nome,
            CPF = api.CPF,
            RG = api.RG,
            Email = api.Email,
            Telefone = api.Telefone,
            Plano = api.Plano,
            Carteira = api.Carteira,
            Nascimento = api.Nascimento,

            // 🔽 flatten do endereço
            Logradouro = api.Endereco.Logradouro,
            Numero = api.Endereco.Numero,
            Bairro = api.Endereco.Bairro,
            Cidade = api.Endereco.Cidade,
            UF = api.Endereco.UF,
            CEP = api.Endereco.CEP
        };

        return ApiResponse<PacienteModel>.Ok(paciente);
    }
    
    public Task<ApiResponse<PacienteModel>> GetByCpf(string cpf)
        => GetAsync<PacienteModel>($"paciente/{cpf}");

    public Task<ApiResponse<bool>> Update(string id, PacienteModel model)
        => PutAsync($"paciente/{id}", model);

    public Task<ApiResponse<bool>> Delete(string id)
        => DeleteAsync($"paciente/{id}");
}
