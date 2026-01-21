using AutoMapper;
using DocAPI.Data;
using DocAPI.Data.Dtos;
using DocAPI.Core.Models;
using DocAPI.Core.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocAPI.Data.Dtos.AgendamentoDtos;

namespace DocAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AgendamentoController : ControllerBase
{
    //private PacienteContext _context;
    private readonly IAgendamentoRepository _repository;
    private readonly IMapper _mapper;

    public AgendamentoController(IAgendamentoRepository repository, IMapper mapper)
    {
        //_context = context;
        _repository = repository;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<IActionResult> GetAgendamentos([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        var agendamentos = await _repository.GetAllAsync(skip, take);
        return Ok(_mapper.Map<IEnumerable<ReadAgendamentoDto>>(agendamentos));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID(string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("O ID do agendamento é obrigatório.");
        
        var agendamento = await _repository.GetByIdAsync(id);
        if (agendamento == null) return NotFound();
        Console.WriteLine($"teste controller:{agendamento.ID}");
        return Ok(_mapper.Map<ReadAgendamentoDto>(agendamento));
    }
    [HttpGet("by-name")]
    public async Task<IActionResult> GetByName([FromQuery]string nome)
    {
        if (string.IsNullOrEmpty(nome))
            return BadRequest("O Nome do paciente é obrigatório.");
        string nomeLimpo = nome.Trim().Replace("\"", "");
        var agendamentos = await _repository.GetByNameAsync(nomeLimpo);
        if (agendamentos == null || !agendamentos.Any()) return NotFound();
        return Ok(_mapper.Map<IEnumerable<ReadAgendamentoDto>>(agendamentos));
    }
    [HttpGet("by-pacientId")]
    public async Task<IActionResult> GetByPacienteId([FromQuery]string pacienteId)
    {
        if (string.IsNullOrEmpty(pacienteId))
            return BadRequest("O pacienteId do agendamento é obrigatório.");
        string pacienteIdLimpo = pacienteId.Trim().Replace("\"", "");
        var agendamentos = await _repository.GetByPacienteIdAsync(pacienteIdLimpo);
        if (agendamentos == null || !agendamentos.Any()) return NotFound();
        return Ok(_mapper.Map<IEnumerable<ReadAgendamentoDto>>(agendamentos));
    }
    [HttpPost]
    public async Task<IActionResult> PostAgendamento([FromBody] CreateAgendamentoDto dto)
    {
        var agendamento = _mapper.Map<Agendamento>(dto);

        await _repository.CreateAsync(agendamento);
        Console.WriteLine($"O agendamento d@ {agendamento.Nome} foi efetuado ");
        Console.WriteLine($"Foi criado o ID: {agendamento.ID}");
        var agendamentoDto = _mapper.Map<ReadAgendamentoDto>(agendamento);
        return CreatedAtAction(nameof(GetByID), new { id = agendamento.ID }, agendamentoDto);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAgendamento(string id, [FromBody] UpdateAgendamentoDto dto)
    {
        Console.WriteLine("teste no Update controller, ativo");
        try
        {
            if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState)
                    {
                        Console.WriteLine($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }

                    return BadRequest(ModelState);
                }
            // 1. Verifica se o ID foi fornecido
            if (string.IsNullOrEmpty(id))
                return BadRequest("O ID do prontuario é obrigatório.");
            if (dto == null)
                return BadRequest("O corpo da requisição está vazio ou inválido.");
            // 2. Mapeia o DTO para a entidade Paciente
            var agendamentoAtualizado = _mapper.Map<Agendamento>(dto);
            agendamentoAtualizado.ID = id.ToString().Trim();

            // 3. Atualiza o paciente na planilha
            await _repository.UpdateAsync(agendamentoAtualizado, id);

            // 4. Retorna sucesso
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar agendamento: {ex.Message}");
            return StatusCode(500, "Erro interno ao atualizar agendamento.");
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAgendamento(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            return BadRequest("O ID do Agendamento é obrigatório.");
            await _repository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao excluir prontuario: {ex.Message}");
            return NotFound("Prontuario não encontrado.");
        }
    }
    
    // método generalista
    // [HttpGet("by-filter")]
    // public async Task<IActionResult> GetByFilter([FromQuery]string filterCondition, string filter )
    // {
    //     if (string.IsNullOrEmpty(filter))
    //         return BadRequest("O filtro selecionado para busca é nulo ou está em branco.");
    //     _repository.FilterValidation()
    //     if (string.IsNullOrEmpty(filterCondition))
    //         return BadRequest("O valor do filtro para busca é nulo ou está em branco.");
    //     string nomeLimpo = nome.Trim().Replace("\"", "");
    //     var agendamentos = await _repository.GetByNameAsync(nomeLimpo);
    //     if (agendamentos == null || !agendamentos.Any()) return NotFound();
    //     return Ok(_mapper.Map<IEnumerable<ReadAgendamentoDto>>(agendamentos));
    // }
}