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
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateProntuarioDto dto)
    {
        var prontuario = _mapper.Map<Prontuario>(dto);

        await _repository.CreateAsync(prontuario);
        Console.WriteLine($"O prontuário d@ {prontuario.DescricaoBasica.NomePaciente} foi efetuado ");
        Console.WriteLine($"foi criado o ID: {prontuario.ID}");
        var prontuarioDto = _mapper.Map<ReadProntuarioDto>(prontuario);
        return CreatedAtAction(nameof(GetByID), new { id = prontuario.ID }, prontuarioDto);
        // return CreatedAtAction(nameof(GetByID), new { id = prontuario.ID }, prontuario);
    }

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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProntuario(string id, [FromBody] UpdateProntuarioDto dto)
    {
        try
        {
            // 1. Verifica se o ID foi fornecido
            if (string.IsNullOrEmpty(id))
                return BadRequest("O ID do prontuario é obrigatório.");
            if (dto == null)
                return BadRequest("O corpo da requisição está vazio ou inválido.");
            // 2. Mapeia o DTO para a entidade Paciente
            var prontuarioAtualizado = _mapper.Map<Prontuario>(dto);

            // 3. Atualiza o paciente na planilha
            await _repository.UpdateAsync(prontuarioAtualizado, id);

            // 4. Retorna sucesso
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar prontuario: {ex.Message}");
            return StatusCode(500, "Erro interno ao atualizar prontuario.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProntuario(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            return BadRequest("O ID do prontuario é obrigatório.");
            await _repository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao excluir prontuario: {ex.Message}");
            return NotFound("Prontuario não encontrado.");
        }
    }
    



}