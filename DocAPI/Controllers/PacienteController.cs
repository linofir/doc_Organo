using AutoMapper;
using DocAPI.Data;
using DocAPI.Data.Dtos;
using DocAPI.Core.Models;
using DocAPI.Core.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace DocAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PacienteController : ControllerBase
{
    //private PacienteContext _context;
    private readonly IPacienteRepository _repository;
    private IMapper _mapper;

    public PacienteController(IPacienteRepository repository, IMapper mapper)
    {
        //_context = context;
        _repository = repository;
        _mapper = mapper;
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreatePacienteDto dto)
    {
        var paciente = _mapper.Map<Paciente>(dto);

        await _repository.CreateAsync(paciente);
        Console.WriteLine($"O cadastro d@ {paciente.Nome} foi efetuado ");
        Console.WriteLine($"foi criado o ID: {paciente.ID}");
        return CreatedAtAction(nameof(GetByID), new { id = paciente.ID }, paciente);
    }
    [HttpGet]
    public async Task<IActionResult> GetPacientes([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        if(_repository == null) return NotFound();
        var pacientes = await _repository.GetAllAsync(skip, take);
        return Ok(_mapper.Map<IEnumerable<ReadPacienteDto>>(pacientes));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID(string id)
    {
        var paciente = await _repository.GetByIdAsync(id);
        if (paciente == null) return NotFound();
        return Ok(_mapper.Map<ReadPacienteDto>(paciente));
    }
    [HttpGet("cpf/{cpf}")]
    public async Task<IActionResult> GetByCpf(string cpf)
    {
        Console.WriteLine("test controller");
        if (string.IsNullOrEmpty(cpf))
            return BadRequest("O cpf da paciente precisa ser fornecido corretamente.");
        string cpfLimpo = cpf.Trim().Replace("\"", "");
        var pacientes = await _repository.GetPacienteByCpfAsync(cpfLimpo);
        if (pacientes == null || !pacientes.Any())
        {
            Console.WriteLine($"Paciente com CPF '{cpf}' não encontrado.");
            return NotFound("Paciente não encontrado."); // HTTP 404 - OK para não encontrado
        }

        if (pacientes.Count() != 1)
        {
            Console.WriteLine($"Erro: Múltiplos pacientes encontrados para o CPF '{cpf}'.");
            // Retorna 409 Conflict com uma mensagem clara
            return Conflict($"Erro: Múltiplos pacientes encontrados para o CPF '{cpf}'. O CPF deve ser único.");
        }
        var pacienteEncontrado = pacientes.First();
        return Ok(_mapper.Map<ReadPacienteDto>(pacienteEncontrado));
    }
    // [HttpGet("/report-id/{id}")]
    // public async Task<IActionResult> GetPatientReportPdf(string id)
    // {
    //     // 1. Validação de entrada (Ex: se o ID não é vazio)
    //     if (string.IsNullOrWhiteSpace(id))
    //     {
    //         return BadRequest("O ID do paciente não pode ser vazio.");
    //     }

    //     try
    //     {
    //         // 2. Chama o repositório que contém a lógica de negócio e as validações
    //         var pdfStream = await _repository.CreateReportByIdAsync(id);

    //         // 3. Retorna o resultado (se nenhuma exceção foi lançada)
    //         string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    //         return File(pdfStream, "application/pdf", $"RelatorioPaciente_{id}.pdf");
    //     }
    //     catch (InvalidOperationException ex) // Captura a exceção de negócio
    //     {
    //         return NotFound(ex.Message); // Retorna 404 Not Found
    //     }
    //     catch (Exception ex)
    //     {
    //         // Logar o erro completo para depuração (ex: via ILogger)
    //         Console.Error.WriteLine($"Erro inesperado ao gerar relatório PDF para paciente ID {id}: {ex.Message} - {ex.StackTrace}");
    //         return StatusCode(500, "Erro interno do servidor ao gerar o relatório."); // Retorna 500 Internal Server Error
    //     }
    // }

    // // Você pode ter um endpoint similar para CPF
    // [HttpGet("{cpf}/report-cpf")]
    // public async Task<IActionResult> GetPatientReportPdfByCpf(string cpf)
    // {
    //     if (string.IsNullOrWhiteSpace(cpf))
    //     {
    //         return BadRequest("O CPF não pode ser vazio.");
    //     }
    //     // Validação de formato de CPF (ex: regex) aqui no controller
    //     // if (!IsValidCpfFormat(cpf)) { return BadRequest("Formato de CPF inválido."); }

    //     try
    //     {
    //         var pdfStream = await _repository.CreateReportByCpfAsync(cpf);
    //         string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    //         return File(pdfStream, "application/pdf", $"RelatorioPaciente_{cpf}.pdf");
    //     }
    //     catch (InvalidOperationException ex)
    //     {
    //         return NotFound(ex.Message);
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.Error.WriteLine($"Erro inesperado ao gerar relatório PDF para CPF {cpf}: {ex.Message} - {ex.StackTrace}");
    //         return StatusCode(500, "Erro interno do servidor ao gerar o relatório.");
    //     }
    // }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePaciente(string id, [FromBody] UpdatePacienteDto dto)
    {
        try
        {
            // 1. Verifica se o ID foi fornecido
            if (string.IsNullOrEmpty(id))
                return BadRequest("O ID do paciente é obrigatório.");

            // 2. Mapeia o DTO para a entidade Paciente
            var pacienteAtualizado = _mapper.Map<Paciente>(dto);

            // 3. Atualiza o paciente na planilha
            await _repository.UpdateAsync(pacienteAtualizado, id);

            // 4. Retorna sucesso
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar paciente: {ex.Message}");
            return StatusCode(500, "Erro interno ao atualizar paciente.");
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaciente(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            return BadRequest("O ID do paciente é obrigatório.");
            await _repository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao excluir paciente: {ex.Message}");
            return NotFound("Paciente não encontrado.");
        }
    }

}