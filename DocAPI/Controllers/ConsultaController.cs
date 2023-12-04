using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DocAPI.Models;
using DocAPI.Data;
using DocAPI.Data.Dtos;
using Microsoft.AspNetCore.JsonPatch;

namespace DocAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ConsultaController : ControllerBase
{
    private PacienteContext _context;
    private IMapper _mapper;

    public ConsultaController(PacienteContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public IActionResult CadastrarConsulta([FromBody] CreateConsultaDto consultaDto)
    {
        Consulta consulta = _mapper.Map<Consulta>(consultaDto);
        _context.Consultas!.Add(consulta);
        _context.SaveChanges();
        // Console.WriteLine($"O cadastro d@ {consulta.Nome} foi efetuado ");
        Console.WriteLine($"foi criado o ID: {consulta.ID}");
       
        return CreatedAtAction(nameof(PesquisaConsultaID), new{id = consulta.ID}, consulta);
    }

    [HttpGet]
    public IActionResult PesquisaConsultas([FromQuery]int skip = 0, [FromQuery]int take = 5)
    {
        if(_context.Consultas == null) return NotFound();
        return Ok(_mapper.Map<List<ReadConsultaDto>>(_context.Consultas.Skip(skip).Take(take).ToList()));
    }

    [HttpGet("secret")]
    public IActionResult PesquisaConsultasSecrets([FromQuery]int skip = 0, [FromQuery]int take = 2)
    {
        if(_context.Consultas == null) return NotFound();
        return Ok(_context.Consultas.Skip(skip).Take(take));
    }

    [HttpGet("{id}")]
    public IActionResult PesquisaConsultaID(string id)
    {
        Consulta consultaEncontrado = _context.Consultas!.FirstOrDefault(consulta => consulta.ID == id)!;
       
        if(consultaEncontrado == null) return NotFound();
        var filmeDto = _mapper.Map<ReadConsultaDto>(consultaEncontrado);
        return Ok(filmeDto);
    }

    [HttpPut("{id}")]
    public IActionResult AlteraConsultaID(string id, [FromBody] UpdateConsultaDto consultaDto)
    {
        Consulta consulta = _context.Consultas!.FirstOrDefault(consulta => consulta.ID == id)!;
        if(consulta == null) return NotFound("O consulta não foi encontrado");
        else
        {
            _mapper.Map(consultaDto, consulta);
            _context.SaveChanges();
            return NoContent();
        }
    }

    [HttpPatch("{id}")]
    public IActionResult AlteraconsultaPatchID(string id, [FromBody] JsonPatchDocument<UpdateConsultaDto> patch)
    {
        Consulta consulta = _context.Consultas!.FirstOrDefault(consulta => consulta.ID == id)!;
        if(consulta == null) return NotFound("O consulta não foi encontrado");
        else
        {
            var consultaAlterando = _mapper.Map<UpdateConsultaDto>(consulta);
            patch.ApplyTo(consultaAlterando, ModelState);
            if(!TryValidateModel(consultaAlterando))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(consultaAlterando, consulta);
            _context.SaveChanges();
            return NoContent();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeletaConsultaID(string id)
    {

        Consulta consultaEncontrado = _context.Consultas!.FirstOrDefault(consulta => consulta.ID == id)!;
       
        if(consultaEncontrado == null) return NotFound();
        _context.Remove(consultaEncontrado);
        _context.SaveChanges();
        return NoContent();
    }

}