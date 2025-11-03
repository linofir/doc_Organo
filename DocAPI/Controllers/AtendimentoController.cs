using AutoMapper;
using DocAPI.Data;
using DocAPI.Data.Dtos.ProntuarioDtos;
using DocAPI.Core.Models;
using DocAPI.Core.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocAPI.Data.Dtos;

namespace DocAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AtendimentoController : ControllerBase
{
    //private PacienteContext _context;
    private readonly IAtendimentoRepository _repository;
    private IMapper _mapper;

    public AtendimentoController(IAtendimentoRepository repository, IMapper mapper)
    {
        //_context = context;
        _repository = repository;
        _mapper = mapper;
    }
    [HttpGet("report-id/{id}")]
    public async Task<IActionResult> GetPatientReportPdf(string id)
    {
        // 1. Validação de entrada (Ex: se o ID não é vazio)
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("O ID do paciente não pode ser vazio.");
        }

        try
        {
            // 2. Chama o repositório que contém a lógica de negócio e as validações
            var pdfStream = await _repository.CreateReportByIdAsync(id);

            // 3. Retorna o resultado (se nenhuma exceção foi lançada)
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            return File(pdfStream, "application/pdf", $"RelatorioPaciente_{id}.pdf");
        }
        catch (InvalidOperationException ex) // Captura a exceção de negócio
        {
            return NotFound(ex.Message); // Retorna 404 Not Found
        }
        catch (Exception ex)
        {
            // Logar o erro completo para depuração (ex: via ILogger)
            Console.Error.WriteLine($"Erro inesperado ao gerar relatório PDF para paciente ID {id}: {ex.Message} - {ex.StackTrace}");
            return StatusCode(500, "Erro interno do servidor ao gerar o relatório."); // Retorna 500 Internal Server Error
        }
    }
    [HttpGet("followUp-id/{id}")]
    public async Task<IActionResult> GetPatientFollowUp(string id)
    {
        // 1. Validação de entrada (Ex: se o ID não é vazio)
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("O ID do paciente não pode ser vazio.");
        }

        try
        {
            // 2. Chama o repositório que contém a lógica de negócio e as validações
            var atendimento = await _repository.CreateReportFollwUpByIdAsync(id);

            // 3. Retorna o resultado (se nenhuma exceção foi lançada)
            return Ok(_mapper.Map<ReadAtendimentoDto>(atendimento));
        }
        catch (InvalidOperationException ex) // Captura a exceção de negócio
        {
            return NotFound(ex.Message); // Retorna 404 Not Found
        }
        catch (Exception ex)
        {
            // Logar o erro completo para depuração (ex: via ILogger)
            Console.Error.WriteLine($"Erro inesperado ao gerar followUp para paciente ID {id}: {ex.Message} - {ex.StackTrace}");
            return StatusCode(500, "Erro interno do servidor ao gerar o followUp."); // Retorna 500 Internal Server Error
        }
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreatePacienteDto dto)
    {
        var atendimento = _mapper.Map<Atendimento>(dto);

        await _repository.CreateAsync(atendimento);
        Console.WriteLine($"O cadastro d@ {atendimento.NomePaciente} foi efetuado ");
        Console.WriteLine($"foi criado o ID: {atendimento.ID}");
        return CreatedAtAction(nameof(GetByID), new { id = atendimento.ID }, atendimento);
    }
    [HttpGet]
    public async Task<IActionResult> GetAtendomentos([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        if(_repository == null) return NotFound();
        var atendomentos = await _repository.GetAllAsync(skip, take);
        return Ok(_mapper.Map<IEnumerable<ReadAtendimentoDto>>(atendomentos));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID(string id)
    {
        var prontuario = await _repository.GetByIdAsync(id);
        if (prontuario == null) return NotFound();
        return Ok(_mapper.Map<ReadAtendimentoDto>(prontuario));
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAtendimento(string id, [FromBody] UpdateAtendimentoDto dto)
    {
        try
        {
            // 1. Verifica se o ID foi fornecido
            if (string.IsNullOrEmpty(id))
                return BadRequest("O ID do Atendimento é obrigatório.");
            if (dto == null)
                return BadRequest("O corpo da requisição está vazio ou inválido.");
            // 2. Mapeia o DTO para a entidade Paciente
            var atendimento = _mapper.Map<Atendimento>(dto);

            // 3. Atualiza o paciente na planilha
            await _repository.UpdateAsync(atendimento, id);

            // 4. Retorna sucesso
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar atendimento: {ex.Message}");
            return StatusCode(500, "Erro interno ao atualizar atendimento.");
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAtendimento(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            return BadRequest("O ID do atendimento é obrigatório.");
            await _repository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao excluir atendimento: {ex.Message}");
            return NotFound("Atendimento não encontrado.");
        }
    }
   
}