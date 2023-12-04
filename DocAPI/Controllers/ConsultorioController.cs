using AutoMapper;
using DocAPI.Data;
using DocAPI.Data.Dtos;
using DocAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace DocAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ConsultorioController : ControllerBase
{
    private PacienteContext _context;
    private IMapper _mapper;

    public ConsultorioController(PacienteContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public IActionResult CadastrarConsultorio([FromBody] CreateConsultorioDto consultorioDto)
    {
        Consultorio consultorio = _mapper.Map<Consultorio>(consultorioDto);
        _context.Consultorios!.Add(consultorio);
        _context.SaveChanges();
        // Console.WriteLine($"O cadastro d@ {consultorio.Nome} foi efetuado ");
        Console.WriteLine($"foi criado o ID: {consultorio.ID}");
       
        return CreatedAtAction(nameof(PesquisaConsultorioID), new{id = consultorio.ID}, consultorio);
    }

    [HttpGet]
    public IActionResult PesquisaConsultorios([FromQuery]int skip = 0, [FromQuery]int take = 2)
    {
        if(_context.Consultorios == null) return NotFound();
        return Ok(_mapper.Map<List<ReadConsultorioDto>>(_context.Consultorios.Skip(skip).Take(take)));
    }

    [HttpGet("secret")]
    public IActionResult PesquisaConsultoriosSecrets([FromQuery]int skip = 0, [FromQuery]int take = 2)
    {
        if(_context.Consultorios == null) return NotFound();
        return Ok(_context.Consultorios.Skip(skip).Take(take));
    }

    [HttpGet("{id}")]
    public IActionResult PesquisaConsultorioID(string id)
    {
        Consultorio consultorioEncontrado = _context.Consultorios!.FirstOrDefault(consultorio => consultorio.ID == id)!;
       
        if(consultorioEncontrado == null) return NotFound();
        var filmeDto = _mapper.Map<ReadConsultorioDto>(consultorioEncontrado);
        return Ok(filmeDto);
    }

    [HttpPut("{id}")]
    public IActionResult AlteraConsultorioID(string id, [FromBody] UpdateConsultorioDto consultorioDto)
    {
        Consultorio consultorio = _context.Consultorios!.FirstOrDefault(consultorio => consultorio.ID == id)!;
        if(consultorio == null) return NotFound("O consultorio não foi encontrado");
        else
        {
            _mapper.Map(consultorioDto, consultorio);
            _context.SaveChanges();
            return NoContent();
        }
    }

    [HttpPatch("{id}")]
    public IActionResult AlteraConsultorioPatchID(string id, [FromBody] JsonPatchDocument<UpdateConsultorioDto> patch)
    {
        Consultorio consultorio = _context.Consultorios!.FirstOrDefault(consultorio => consultorio.ID == id)!;
        if(consultorio == null) return NotFound("O consultorio não foi encontrado");
        else
        {
            var consultorioAlterando = _mapper.Map<UpdateConsultorioDto>(consultorio);
            patch.ApplyTo(consultorioAlterando, ModelState);
            if(!TryValidateModel(consultorioAlterando))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(consultorioAlterando, consultorio);
            _context.SaveChanges();
            return NoContent();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeletaConsultorioID(string id)
    {

        Consultorio consultorioEncontrado = _context.Consultorios!.FirstOrDefault(consultorio => consultorio.ID == id)!;
       
        if(consultorioEncontrado == null) return NotFound();
        _context.Remove(consultorioEncontrado);
        _context.SaveChanges();
        return NoContent();
    }
}