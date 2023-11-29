using AutoMapper;
using DocAPI.Data;
using DocAPI.Data.Dtos;
using DocAPI.Models;
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
        Console.WriteLine("método get");
        

        if(_context.Pacientes == null) return NotFound();
        return Ok(_context.Pacientes.Skip(skip).Take(take));
    }

    [HttpGet("{id}")]
    public IActionResult PesquisaPacienteID(string id)
    {
        Console.WriteLine("método ID");
        
        Paciente pacienteEncontrado = _context.Pacientes!.FirstOrDefault(paciente => paciente.ID == id)!;
       
        Console.WriteLine(pacienteEncontrado);
        if(pacienteEncontrado == null) return NotFound();
        return Ok(pacienteEncontrado);
    }
}