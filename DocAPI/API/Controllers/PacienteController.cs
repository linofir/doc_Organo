using AutoMapper;
using DocAPI.Data.Dtos;
using DocAPI.Core.Entities;
using DocAPI.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PacienteController : ControllerBase
{
    private readonly IPacienteRepository _repository;
    private readonly IMapper _mapper;

    public PacienteController(IPacienteRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreatePacienteDto dto)
    {
        var paciente = _mapper.Map<Paciente>(dto);

        try
        {
            await _repository.CreateAsync(paciente);
        }
        catch (DbUpdateException ex) when (IsDuplicateCpfViolation(ex))
        {
            return Conflict("Paciente com CPF duplicado.");
        }

        var readDto = _mapper.Map<ReadPacienteDto>(paciente);
        return CreatedAtAction(nameof(GetByID), new { id = paciente.ID }, readDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetPacientes([FromQuery] int skip = 0, [FromQuery] int take = 100)
    {
        if (skip < 0 || take <= 0)
            return BadRequest("Parâmetros de paginação inválidos.");

        var pacientes = await _repository.GetAllAsync(skip, take);
        return Ok(_mapper.Map<IEnumerable<ReadPacienteDto>>(pacientes));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByNome(
        [FromQuery] string nome,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return BadRequest("O nome da paciente precisa ser fornecido corretamente.");

        if (skip < 0 || take <= 0)
            return BadRequest("Parâmetros de paginação inválidos.");

        var pacientes = await _repository.GetPacienteByNomeAsync(nome.Trim(), skip, take);
        return Ok(_mapper.Map<IEnumerable<ReadPacienteDto>>(pacientes));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID(Guid id)
    {
        var paciente = await _repository.GetByIdAsync(id);
        if (paciente == null)
            return NotFound();

        return Ok(_mapper.Map<ReadPacienteDto>(paciente));
    }

    [HttpGet("cpf/{cpf}")]
    public async Task<IActionResult> GetByCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return BadRequest("O cpf da paciente precisa ser fornecido corretamente.");

        var cpfLimpo = cpf.Trim().Replace("\"", "");
        var pacientes = await _repository.GetPacienteByCpfAsync(cpfLimpo);
        if (pacientes.Count == 0)
            return NotFound();

        if (pacientes.Count != 1)
            return Conflict("Erro de integridade: múltiplos registros para o mesmo CPF.");

        return Ok(_mapper.Map<ReadPacienteDto>(pacientes[0]));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePaciente(Guid id, [FromBody] UpdatePacienteDto dto)
    {
        var pacienteAtualizado = _mapper.Map<Paciente>(dto);

        try
        {
            await _repository.UpdateAsync(pacienteAtualizado, id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (DbUpdateException ex) when (IsDuplicateCpfViolation(ex))
        {
            return Conflict("Paciente com CPF duplicado.");
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaciente(Guid id)
    {
        try
        {
            await _repository.DeleteAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }

    private static bool IsDuplicateCpfViolation(DbUpdateException ex)
    {
        var message = ex.InnerException?.Message ?? ex.Message;
        return message.Contains("IX_Paciente_CPF", StringComparison.OrdinalIgnoreCase)
            || message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase)
            || message.Contains("UNIQUE KEY", StringComparison.OrdinalIgnoreCase);
    }
}
