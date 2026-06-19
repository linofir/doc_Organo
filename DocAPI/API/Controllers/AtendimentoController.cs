using AutoMapper;
using DocAPI.Data.Dtos.Atendimento;
using DocAPI.Core.Entities;
using DocAPI.Core.Interfaces.Repositories;
using DocAPI.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DocAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AtendimentoController : ControllerBase
{
    private readonly IAtendimentoRepository _repository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IMapper _mapper;

    public AtendimentoController(
        IAtendimentoRepository repository,
        IPacienteRepository pacienteRepository,
        IMapper mapper)
    {
        _repository = repository;
        _pacienteRepository = pacienteRepository;
        _mapper = mapper;
    }

    [HttpGet("report-id/{id}")]
    public IActionResult GetPatientReportPdf(Guid id) =>
        StatusCode(StatusCodes.Status501NotImplemented);

    [HttpGet("followUp-id/{id}")]
    public IActionResult GetPatientFollowUp(Guid id) =>
        StatusCode(StatusCodes.Status501NotImplemented);

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateAtendimentoDto dto)
    {
        if (dto == null)
            return BadRequest("O corpo da requisição está vazio ou inválido.");

        var paciente = await _pacienteRepository.GetByIdAsync(dto.PacienteId);
        if (paciente == null)
            return NotFound();

        var atendimento = new Atendimento(dto.PacienteId, dto.MensagemParaMedico);
        await _repository.CreateAsync(atendimento);

        var readDto = _mapper.Map<ReadAtendimentoDto>(atendimento);
        return CreatedAtAction(nameof(GetByID), new { id = atendimento.Id }, readDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAtendomentos([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        if (skip < 0 || take <= 0)
            return BadRequest("Parâmetros de paginação inválidos.");

        var atendimentos = await _repository.GetAllAsync(skip, take);
        return Ok(_mapper.Map<IEnumerable<ReadAtendimentoDto>>(atendimentos));
    }

    [HttpGet("paciente/{pacienteId}")]
    public async Task<IActionResult> GetByPacienteId(Guid pacienteId)
    {
        var atendimentos = await _repository.GetByPacienteIdAsync(pacienteId);
        return Ok(_mapper.Map<IEnumerable<ReadAtendimentoDto>>(atendimentos));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID(Guid id)
    {
        var atendimento = await _repository.GetByIdAsync(id);
        if (atendimento == null)
            return NotFound();

        return Ok(_mapper.Map<ReadAtendimentoDto>(atendimento));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAtendimento(Guid id, [FromBody] UpdateAtendimentoDto dto)
    {
        if (dto == null)
            return BadRequest("O corpo da requisição está vazio ou inválido.");

        var atendimentoAtualizado = new Atendimento(Guid.Empty, dto.MensagemParaMedico);

        try
        {
            await _repository.UpdateAsync(atendimentoAtualizado, id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAtendimento(Guid id)
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
}
