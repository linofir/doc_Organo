using AutoMapper;
using DocAPI.Data;
using DocAPI.Data.Dtos;
using DocAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace DocAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PacienteController : ControllerBase
{
    private PacienteContext _context;
    private IMapper _mapper;

    public PacienteController(PacienteContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public IActionResult CadastrarPaciente([FromBody] CreatePacienteDto pacienteDto)
    {
        Paciente paciente = _mapper.Map<Paciente>(pacienteDto);
        _context.Pacientes!.Add(paciente);
        _context.SaveChanges();
        Console.WriteLine($"O cadastro d@ {paciente.Nome} foi efetuado ");
        Console.WriteLine($"foi criado o ID: {paciente.ID}");
       
        return CreatedAtAction(nameof(PesquisaPacienteID), new{id = paciente.ID}, paciente);
    }

    [HttpGet]
    public IActionResult PesquisaPacientes([FromQuery]int skip = 0, [FromQuery]int take = 2)
    {
        if(_context.Pacientes == null) return NotFound();
        return Ok(_mapper.Map<List<ReadPacienteDto>>(_context.Pacientes.Skip(skip).Take(take)));
    }

    [HttpGet("secret")]
    public IActionResult PesquisaPacientesSecrets([FromQuery]int skip = 0, [FromQuery]int take = 2)
    {
        if(_context.Pacientes == null) return NotFound();
        return Ok(_context.Pacientes.Skip(skip).Take(take));
    }

    [HttpGet("{id}")]
    public IActionResult PesquisaPacienteID(string id)
    {
        Paciente pacienteEncontrado = _context.Pacientes!.FirstOrDefault(paciente => paciente.ID == id)!;
       
        if(pacienteEncontrado == null) return NotFound();
        var filmeDto = _mapper.Map<ReadPacienteDto>(pacienteEncontrado);
        return Ok(filmeDto);
    }

    [HttpPut("{id}")]
    public IActionResult AlteraPacienteID(string id, [FromBody] UpdatePacienteDto pacienteDto)
    {
        Paciente paciente = _context.Pacientes!.FirstOrDefault(paciente => paciente.ID == id)!;
        if(paciente == null) return NotFound("O paciente não foi encontrado");
        else
        {
            _mapper.Map(pacienteDto, paciente);
            _context.SaveChanges();
            return NoContent();
        }
    }

    [HttpPatch("{id}")]
    public IActionResult AlteraPacientePatchID(string id, [FromBody] JsonPatchDocument<UpdatePacienteDto> patch)
    {
        Paciente paciente = _context.Pacientes!.FirstOrDefault(paciente => paciente.ID == id)!;
        if(paciente == null) return NotFound("O paciente não foi encontrado");
        else
        {
            var pacienteAlterando = _mapper.Map<UpdatePacienteDto>(paciente);
            patch.ApplyTo(pacienteAlterando, ModelState);
            if(!TryValidateModel(pacienteAlterando))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(pacienteAlterando, paciente);
            _context.SaveChanges();
            return NoContent();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeletaPacienteID(string id)
    {

        Paciente pacienteEncontrado = _context.Pacientes!.FirstOrDefault(paciente => paciente.ID == id)!;
       
        if(pacienteEncontrado == null) return NotFound();
        _context.Remove(pacienteEncontrado);
        _context.SaveChanges();
        return NoContent();
    }
}