using AutoMapper;
using DocAPI.Data;
using DocAPI.Data.Dtos;
using DocAPI.Core.Models;
using DocAPI.Core.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    // [HttpPost]
    // public IActionResult CadastrarProntuario([FromBody] CreateProntuarioDto pacienteDto)
    // {
    //     Paciente paciente = _mapper.Map<Paciente>(pacienteDto);
    //     _context.Pacientes!.Add(paciente);
    //     _context.SaveChanges();
    //     Console.WriteLine($"O cadastro d@ {paciente.Nome} foi efetuado ");
    //     Console.WriteLine($"foi criado o ID: {paciente.ID}");
       
    //     return CreatedAtAction(nameof(GetByID), new{id = paciente.ID}, paciente);
    // }

    [HttpGet]
    public async Task<IActionResult> GetPacientes([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        if(_repository == null) return NotFound();
        var pacientes = await _repository.GetAllAsync(skip, take);
        return Ok(_mapper.Map<IEnumerable<ReadPacienteDto>>(pacientes));
    }
    // [HttpGet]
    // public IActionResult GetPacientes([FromQuery]int skip = 0, [FromQuery]int take = 2)
    // {
    //     if(_context.Pacientes == null) return NotFound();
    //     return Ok(_mapper.Map<List<ReadPacienteDto>>(_context.Pacientes.Skip(skip).Take(take).ToList()));
    // }

    // [HttpGet("secret")]
    // public IActionResult GetPacientesSecrets([FromQuery]int skip = 0, [FromQuery]int take = 2)
    // {
    //     if(_context.Pacientes == null) return NotFound();
    //     return Ok(_context.Pacientes.Skip(skip).Take(take));
    // }
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
        var paciente = await _repository.GetByCpfAsync(cpf);
        if (paciente == null) return NotFound();
        return Ok(_mapper.Map<ReadPacienteDto>(paciente));
    }

    // [HttpGet("{id}")]
    // public IActionResult GetByID(string id)
    // {
    //     Paciente pacienteEncontrado = _context.Pacientes!.FirstOrDefault(paciente => paciente.ID == id)!;
       
    //     if(pacienteEncontrado == null) return NotFound();
    //     var filmeDto = _mapper.Map<ReadPacienteDto>(pacienteEncontrado);
    //     return Ok(filmeDto);
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


    // [HttpPut("{id}")]
    // public IActionResult AlteraPacienteID(string id, [FromBody] UpdatePacienteDto pacienteDto)
    // {
    //     Paciente paciente = _context.Pacientes!.FirstOrDefault(paciente => paciente.ID == id)!;
    //     if(paciente == null) return NotFound("O paciente não foi encontrado");
    //     else
    //     {
    //         _mapper.Map(pacienteDto, paciente);
    //         _context.SaveChanges();
    //         return NoContent();
    //     }
    // }

    // [HttpPatch("{id}")]
    // public IActionResult AlteraPacientePatchID(string id, [FromBody] JsonPatchDocument<UpdatePacienteDto> patch)
    // {
    //     Paciente paciente = _context.Pacientes!.FirstOrDefault(paciente => paciente.ID == id)!;
    //     if(paciente == null) return NotFound("O paciente não foi encontrado");
    //     else
    //     {
    //         var pacienteAlterando = _mapper.Map<UpdatePacienteDto>(paciente);
    //         patch.ApplyTo(pacienteAlterando, ModelState);
    //         if(!TryValidateModel(pacienteAlterando))
    //         {
    //             return ValidationProblem(ModelState);
    //         }
    //         _mapper.Map(pacienteAlterando, paciente);
    //         _context.SaveChanges();
    //         return NoContent();
    //     }
    // }
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
    


    // [HttpDelete("{id}")]
    // public IActionResult DeletaPacienteID(string id)
    // {

    //     Paciente pacienteEncontrado = _context.Pacientes!.FirstOrDefault(paciente => paciente.ID == id)!;
       
    //     if(pacienteEncontrado == null) return NotFound();
    //     _context.Remove(pacienteEncontrado);
    //     _context.SaveChanges();
    //     return NoContent();
    // }
}