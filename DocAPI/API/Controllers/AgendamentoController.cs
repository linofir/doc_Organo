using AutoMapper;
using DocAPI.Core.Entities;
using DocAPI.Core.Interfaces.Repositories;
using DocAPI.Data.Dtos.AgendamentoDtos;
using DocAPI.Infrastructure.SqlDb.Context;
using DocAPI.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AgendamentoController : ControllerBase
{
    private readonly IAgendamentoRepository _repository;
    private readonly IMapper _mapper;
    private readonly IAtendimentoRepository _atendimentoRepository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly DocDbContext _context;

    public AgendamentoController(
        IAgendamentoRepository repository,
        IMapper mapper,
        IAtendimentoRepository atendimentoRepository,
        IPacienteRepository pacienteRepository,
        DocDbContext context)
    {
        _repository = repository;
        _mapper = mapper;
        _atendimentoRepository = atendimentoRepository;
        _pacienteRepository = pacienteRepository;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAgendamentos([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        var agendamentos = await _repository.GetAllAsync(skip, take);
        return Ok(_mapper.Map<IEnumerable<ReadAgendamentoDto>>(agendamentos));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByID(Guid id)
    {
        var agendamento = await _repository.GetByIdAsync(id);
        if (agendamento == null) return NotFound();
        return Ok(_mapper.Map<ReadAgendamentoDto>(agendamento));
    }

    [HttpGet("by-name")]
    public async Task<IActionResult> GetByName([FromQuery] string nome)
    {
        if (string.IsNullOrEmpty(nome))
            return BadRequest("O nome é obrigatório.");

        string nomeLimpo = nome.Trim().Replace("\"", "");
        var agendamentos = await _repository.GetByNameAsync(nomeLimpo);
        return Ok(_mapper.Map<IEnumerable<ReadAgendamentoDto>>(agendamentos));
    }

    [HttpGet("by-pacientId")]
    public async Task<IActionResult> GetByPacienteId([FromQuery] Guid pacienteId)
    {
        var agendamentos = await _repository.GetByPacienteIdAsync(pacienteId);
        return Ok(_mapper.Map<IEnumerable<ReadAgendamentoDto>>(agendamentos));
    }

    [HttpPost]
    public async Task<IActionResult> PostAgendamento([FromBody] CreateAgendamentoDto dto)
    {
        // FK validation: AtendimentoId
        var atendimento = await _atendimentoRepository.GetByIdAsync(dto.AtendimentoId);
        if (atendimento == null)
            return NotFound(new
            {
                title = "Foreign key reference not found",
                status = 404,
                field = "atendimentoId",
                detail = "The referenced resource was not found or has been removed."
            });

        // FK validation: InternacaoId
        var internacao = await _context.Set<Internacao>()
            .FirstOrDefaultAsync(i => i.ID == dto.InternacaoId);
        if (internacao == null)
            return NotFound(new
            {
                title = "Foreign key reference not found",
                status = 404,
                field = "internacaoId",
                detail = "The referenced resource was not found or has been removed."
            });

        // FK validation: PacienteId (optional)
        if (dto.PacienteId.HasValue)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(dto.PacienteId.Value);
            if (paciente == null)
                return NotFound(new
                {
                    title = "Foreign key reference not found",
                    status = 404,
                    field = "pacienteId",
                    detail = "The referenced resource was not found or has been removed."
                });
        }

        var agendamento = new Agendamento(
            internacaoId: dto.InternacaoId,
            atendimentoId: dto.AtendimentoId,
            data: dto.Data,
            horario: dto.Horario,
            pacienteID: dto.PacienteId,
            nome: dto.Nome,
            aviso: dto.Aviso,
            local: dto.Local,
            sala: dto.Sala,
            dataConsulta: dto.DataConsulta,
            instrucaoStatus: dto.InstrucaoStatus,
            atestadoStatus: dto.AtestadoStatus,
            senhaAgendamento: dto.SenhaAgendamento);

        await _repository.CreateAsync(agendamento);

        var agendamentoDto = _mapper.Map<ReadAgendamentoDto>(agendamento);
        return CreatedAtAction(nameof(GetByID), new { id = agendamento.ID }, agendamentoDto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAgendamento(Guid id, [FromBody] UpdateAgendamentoDto dto)
    {
        var agendamento = await _repository.GetByIdAsync(id);
        if (agendamento == null) return NotFound();

        agendamento.Update(
            nome: dto.Nome,
            aviso: dto.Aviso,
            data: dto.Data,
            horario: dto.Horario,
            local: dto.Local,
            sala: dto.Sala,
            status: dto.Status,
            instrucaoStatus: dto.InstrucaoStatus,
            atestadoStatus: dto.AtestadoStatus,
            dataConsulta: dto.DataConsulta);

        if (dto.SenhaAgendamento is not null || dto.SenhaAgendamento != agendamento.SenhaAgendamento)
        {
            agendamento.SetSenha(dto.SenhaAgendamento);
        }

        await _repository.UpdateAsync(agendamento, id);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAgendamento(Guid id)
    {
        var agendamento = await _repository.GetByIdAsync(id);
        if (agendamento == null) return NotFound();

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}