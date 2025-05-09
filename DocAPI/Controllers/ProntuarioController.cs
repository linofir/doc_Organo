using AutoMapper;
using DocAPI.Data;
using DocAPI.Data.Dtos.ProntuarioDtos;
using DocAPI.Core.Models;
using DocAPI.Core.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProntuarioController : ControllerBase
{
    //private PacienteContext _context;
    private readonly IProntuarioRepository _repository;
    private IMapper _mapper;

    public ProntuarioController(IProntuarioRepository repository, IMapper mapper)
    {
        //_context = context;
        _repository = repository;
        _mapper = mapper;
    }
    // [HttpPost]
    // public async Task<IActionResult> Post([FromBody] CreatePacienteDto dto)
    // {
    //     var paciente = _mapper.Map<Paciente>(dto);

    //     await _repository.CreateAsync(paciente);
    //     Console.WriteLine($"O cadastro d@ {paciente.Nome} foi efetuado ");
    //     Console.WriteLine($"foi criado o ID: {paciente.ID}");
    //     return CreatedAtAction(nameof(GetByID), new { id = paciente.ID }, paciente);
    // }

    [HttpGet]
    public async Task<IActionResult> GetProntuarios([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        if(_repository == null) return NotFound();
        var prontuarios = await _repository.GetAllAsync(skip, take);
        return Ok(_mapper.Map<IEnumerable<ReadProntuarioDto>>(prontuarios));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID(string id)
    {
        var prontuario = await _repository.GetByIdAsync(id);
        if (prontuario == null) return NotFound();
        return Ok(_mapper.Map<ReadProntuarioDto>(prontuario));
    }

    // [HttpPut("{id}")]
    // public async Task<IActionResult> UpdatePaciente(string id, [FromBody] UpdatePacienteDto dto)
    // {
    //     try
    //     {
    //         // 1. Verifica se o ID foi fornecido
    //         if (string.IsNullOrEmpty(id))
    //             return BadRequest("O ID do paciente é obrigatório.");

    //         // 2. Mapeia o DTO para a entidade Paciente
    //         var pacienteAtualizado = _mapper.Map<Paciente>(dto);

    //         // 3. Atualiza o paciente na planilha
    //         await _repository.UpdateAsync(pacienteAtualizado, id);

    //         // 4. Retorna sucesso
    //         return NoContent();
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Erro ao atualizar paciente: {ex.Message}");
    //         return StatusCode(500, "Erro interno ao atualizar paciente.");
    //     }
    // }

    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeletePaciente(string id)
    // {
    //     try
    //     {
    //         if (string.IsNullOrEmpty(id))
    //         return BadRequest("O ID do paciente é obrigatório.");
    //         await _repository.DeleteAsync(id);
    //         return NoContent();
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Erro ao excluir paciente: {ex.Message}");
    //         return NotFound("Paciente não encontrado.");
    //     }
    // }
    



}