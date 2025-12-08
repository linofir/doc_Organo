using DocFront.Utils;
using DocFront.Models;
//o ApiService já cria o HttpClient configurado para a API base.
public class PacienteService : ApiService
{
    public PacienteService(IHttpClientFactory factory) 
        : base(factory) {}

    public Task<ApiResponse<List<PacienteModel>>> GetAll()
        => GetAsync<List<PacienteModel>>("paciente");

    public Task<ApiResponse<PacienteModel>> GetById(string id)
        => GetAsync<PacienteModel>($"paciente/{id}");
    
    public Task<ApiResponse<PacienteModel>> GetByCpf(string cpf)
        => GetAsync<PacienteModel>($"paciente/{cpf}");
    public Task<ApiResponse<bool>> Create(PacienteModel model)
        => PostAsync("paciente", model);

    public Task<ApiResponse<bool>> Update(string id, PacienteModel model)
        => PutAsync($"paciente/{id}", model);

    public Task<ApiResponse<bool>> Delete(string id)
        => DeleteAsync($"paciente/{id}");
}
