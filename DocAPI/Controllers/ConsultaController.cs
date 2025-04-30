// using AutoMapper;
// using Microsoft.AspNetCore.Mvc;
// using DocAPI.Core.Models;
// using DocAPI.Data;
// using DocAPI.Data.Dtos;
// using Microsoft.AspNetCore.JsonPatch;

// namespace DocAPI.Controllers;

// [ApiController]
// [Route("[controller]")]
// public class ConsultaController : ControllerBase
// {
//     private PacienteContext _context;
//     private IMapper _mapper;

//     public ConsultaController(PacienteContext context, IMapper mapper)
//     {
//         _context = context;
//         _mapper = mapper;
//     }

//     // [HttpPost]
//     // public IActionResult CadastrarConsulta([FromBody] CreateConsultaDto consultaDto)
//     // {
//     //     Consulta consulta = _mapper.Map<Consulta>(consultaDto);
//     //     _context.Consultas!.Add(consulta);
//     //     _context.SaveChanges();
//     //     // Console.WriteLine($"O cadastro d@ {consulta.Nome} foi efetuado ");
//     //     Console.WriteLine($"foi criado o ID: {consulta.PacienteID}, consultorio: {consulta.ConsultorioID} ");
       
//     //     return CreatedAtAction(nameof(PesquisaConsultaID), new{pacienteId = consulta.PacienteID, consultorioId = consulta.ConsultorioID} , consulta);
//     // }

//     // [HttpGet]
//     // public IActionResult PesquisaConsultas([FromQuery]int skip = 0, [FromQuery]int take = 5) 
//     // {
//     //     if(_context.Consultas == null) return NotFound();
//     //     return Ok(_mapper.Map<List<ReadConsultaDto>>(_context.Consultas.Skip(skip).Take(take).ToList()));
//     // }

//     [HttpGet("secret")]
//     public IActionResult PesquisaConsultasSecrets([FromQuery]int skip = 0, [FromQuery]int take = 2)
//     {
//         if(_context.Consultas == null) return NotFound();
//         return Ok(_context.Consultas.Skip(skip).Take(take));
//     }

//     [HttpGet("{pacienteId}/{consultorioId}")]
//     public IActionResult PesquisaConsultaID(string pacienteId, string consultorioId)
//     {
//         Consulta consultaEncontrado = _context.Consultas!.FirstOrDefault(consulta => consulta.PacienteID == pacienteId && consulta.ConsultorioID == consultorioId)!;
       
//         if(consultaEncontrado == null) return NotFound();
//         var filmeDto = _mapper.Map<ReadConsultaDto>(consultaEncontrado);
//         return Ok(filmeDto);
//     }

//     [HttpPut("{pacienteId}/{consultorioId}")]
//     public IActionResult AlteraConsultaID(string pacienteId, string consultorioId, [FromBody] UpdateConsultaDto consultaDto)
//     {
//         Consulta consulta = _context.Consultas!.FirstOrDefault(consulta => consulta.PacienteID == pacienteId && consulta.ConsultorioID == consultorioId)!;
//         if(consulta == null) return NotFound("O consulta não foi encontrado");
//         else
//         {
//             _mapper.Map(consultaDto, consulta);
//             _context.SaveChanges();
//             return NoContent();
//         }
//     }

//     [HttpPatch("{pacienteId}/{consultorioId}")]
//     public IActionResult AlteraconsultaPatchID(string pacienteId, string consultorioId, [FromBody] JsonPatchDocument<UpdateConsultaDto> patch)
//     {
//         Consulta consulta = _context.Consultas!.FirstOrDefault(consulta => consulta.PacienteID == pacienteId && consulta.ConsultorioID == consultorioId)!;
//         if(consulta == null) return NotFound("O consulta não foi encontrado");
//         else
//         {
//             var consultaAlterando = _mapper.Map<UpdateConsultaDto>(consulta);
//             patch.ApplyTo(consultaAlterando, ModelState);
//             if(!TryValidateModel(consultaAlterando))
//             {
//                 return ValidationProblem(ModelState);
//             }
//             _mapper.Map(consultaAlterando, consulta);
//             _context.SaveChanges();
//             return NoContent();
//         }
//     }

//     [HttpDelete("{pacienteId}/{consultorioId}")]
//     public IActionResult DeletaConsultaID(string pacienteId, string consultorioId)
//     {

//         Consulta consultaEncontrado = _context.Consultas!.FirstOrDefault(consulta => consulta.PacienteID == pacienteId && consulta.ConsultorioID == consultorioId)!;
       
//         if(consultaEncontrado == null) return NotFound();
//         _context.Remove(consultaEncontrado);
//         _context.SaveChanges();
//         return NoContent();
//     }

// }