using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocAPI.Core.Entities;
using DocAPI.Core.Interfaces.Repositories;
using DocAPI.Data.Dtos.ProntuarioDtos;
using DocAPI.Infrastructure.SqlDb.Context;
using DocAPI.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProntuarioController : ControllerBase
{
    private readonly IProntuarioRepository _repository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IAtendimentoRepository _atendimentoRepository;
    private readonly DocDbContext _context;
    private readonly IMapper _mapper;

    public ProntuarioController(
        IProntuarioRepository repository,
        IPacienteRepository pacienteRepository,
        IAtendimentoRepository atendimentoRepository,
        DocDbContext context,
        IMapper mapper)
    {
        _repository = repository;
        _pacienteRepository = pacienteRepository;
        _atendimentoRepository = atendimentoRepository;
        _context = context;
        _mapper = mapper;
    }

    // ── Create v1 ──────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> PostProntuario([FromBody] CreateProntuarioDto dto)
    {
        // FK validation: Atendimento exists, not soft-deleted, PacienteId match
        var atendimento = await _atendimentoRepository.GetByIdAsync(dto.AtendimentoId);
        if (atendimento == null)
            return NotFound();

        if (atendimento.PacienteId != dto.PacienteId)
            return NotFound();

        // Load Paciente for identity snapshot
        var paciente = await _pacienteRepository.GetByIdAsync(dto.PacienteId);
        if (paciente == null)
            return NotFound();

        // CID validation when Internacao present (REQ-010)
        if (dto.SolicitacaoInternacao != null)
        {
            var cidExists = await _context.CIDs
                .AnyAsync(c => c.Codigo == dto.SolicitacaoInternacao.CIDCodigo);
            if (!cidExists)
                return BadRequest("CID não encontrado para a internação solicitada.");
        }

        // Build clinical sections from DTOs
        var descBasica = MapDtoToDescricaoBasica(dto.DescricaoBasica, paciente);
        var ago = MapDtoToAGO(dto.AGO);
        var antecedentes = MapDtoToAntecedentes(dto.Antecedentes);
        var antFamiliares = MapDtoToAntecedentesFamiliares(dto.AntecedentesFamiliares);
        var posOp = MapDtoToPosOp(dto.PosOperatorio);
        var acoesCD = MapDtoToAcoesCD(dto.CD);
        var exames = MapDtoToExames(dto.Exames);
        var internacao = MapDtoToInternacao(dto.SolicitacaoInternacao);

        // Create aggregate
        var prontuario = new Prontuario(dto.PacienteId, dto.AtendimentoId);
        prontuario.AplicarCriacao(
            dto.DataConsulta, dto.Tipo, dto.InformacoesExtras,
            descBasica, ago, antecedentes, antFamiliares,
            posOp, acoesCD, exames, internacao);

        await _repository.AddAsync(prontuario);

        var readDto = _mapper.Map<ReadProntuarioDto>(prontuario);
        return CreatedAtAction(nameof(GetByID), new { id = prontuario.ID }, readDto);
    }

    // ── PDF (out of scope) ─────────────────────────────────

    [HttpPost("from-pdf")]
    public IActionResult PostFromPDF()
        => StatusCode(501);

    // ── Reads ──────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> GetProntuarios([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        var prontuarios = await _repository.GetAllAsync(skip, take);
        return Ok(_mapper.Map<IEnumerable<ReadProntuarioDto>>(prontuarios));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByID(Guid id)
    {
        var prontuario = await _repository.GetByIdAsync(id);
        if (prontuario == null) return NotFound();
        return Ok(_mapper.Map<ReadProntuarioDto>(prontuario));
    }

    [HttpGet("paciente/{pacienteId:guid}")]
    public async Task<IActionResult> GetByPacienteId(Guid pacienteId)
    {
        var prontuarios = await _repository.GetByPacienteIdAsync(pacienteId);
        return Ok(_mapper.Map<IEnumerable<ReadProntuarioDto>>(prontuarios));
    }

    [HttpGet("paciente/{pacienteId:guid}/atual")]
    public async Task<IActionResult> GetLatestByPacienteId(Guid pacienteId)
    {
        var prontuario = await _repository.GetLatestByPacienteIdAsync(pacienteId);
        if (prontuario == null) return NotFound();
        return Ok(_mapper.Map<ReadProntuarioDto>(prontuario));
    }

    [HttpGet("atendimento/{atendimentoId:guid}/atual")]
    public async Task<IActionResult> GetLatestByAtendimentoId(Guid atendimentoId)
    {
        var prontuario = await _repository.GetLatestByAtendimentoIdAsync(atendimentoId);
        if (prontuario == null) return NotFound();
        return Ok(_mapper.Map<ReadProntuarioDto>(prontuario));
    }

    // ── Correction (PUT) ───────────────────────────────────

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProntuario(Guid id, [FromBody] UpdateProntuarioDto dto)
    {
        if (dto == null)
            return BadRequest();

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        existing.AplicarCorrecao(
            dto.InformacoesExtras,
            dto.Profissao,
            dto.Religiao,
            dto.AtividadeFisica);

        await _repository.UpdateAsync(existing);
        return NoContent();
    }

    // ── Clinical Evolution (POST /versoes) ─────────────────

    [HttpPost("{id:guid}/versoes")]
    public async Task<IActionResult> CriarNovaVersao(Guid id, [FromBody] CreateVersaoProntuarioDto dto)
    {
        var source = await _repository.GetByIdAsync(id);
        if (source == null)
            return NotFound();

        // CID validation when Internacao present (REQ-010)
        if (dto.SolicitacaoInternacao != null)
        {
            var cidExists = await _context.CIDs
                .AnyAsync(c => c.Codigo == dto.SolicitacaoInternacao.CIDCodigo);
            if (!cidExists)
                return BadRequest("CID não encontrado para a internação solicitada.");
        }

        // D-02: Application obtains max Versao from persistence lookup
        var maxVersao = await _repository.GetMaxVersaoForPacienteAsync(source.PacienteId);
        var nextVersao = maxVersao + 1;

        // Build clinical sections
        var paciente = await _pacienteRepository.GetByIdAsync(source.PacienteId);
        var descBasica = MapDtoToDescricaoBasica(dto.DescricaoBasica, paciente);
        var ago = MapDtoToAGO(dto.AGO);
        var antecedentes = MapDtoToAntecedentes(dto.Antecedentes);
        var antFamiliares = MapDtoToAntecedentesFamiliares(dto.AntecedentesFamiliares);
        var posOp = MapDtoToPosOp(dto.PosOperatorio);
        var acoesCD = MapDtoToAcoesCD(dto.CD);
        var exames = MapDtoToExames(dto.Exames);
        var internacao = MapDtoToInternacao(dto.SolicitacaoInternacao);

        Prontuario novaVersao;
        try
        {
            novaVersao = source.CriarNovaVersao(
                nextVersao, dto.DataConsulta, dto.Tipo, dto.InformacoesExtras,
                descBasica, ago, antecedentes, antFamiliares,
                posOp, acoesCD, exames, internacao);
        }
        catch (InvalidOperationException)
        {
            return Conflict();
        }

        try
        {
            await _repository.AddAsync(novaVersao);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true
               || ex.InnerException?.Message.Contains("IX_Prontuario", StringComparison.OrdinalIgnoreCase) == true)
        {
            // D-06: concurrent evolution → 409
            return Conflict();
        }

        var readDto = _mapper.Map<ReadProntuarioDto>(novaVersao);
        return CreatedAtAction(nameof(GetByID), new { id = novaVersao.ID }, readDto);
    }

    // ── Soft Delete ────────────────────────────────────────

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProntuario(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        existing.MarcarComoExcluido();
        await _repository.DeleteAsync(existing);
        return NoContent();
    }

    // ── DTO → Entity mapping helpers (no AutoMapper on write) ──

    private static DescricaoBasica MapDtoToDescricaoBasica(
        DescricaoBasicaDto? dto, Paciente? paciente)
    {
        var nome = paciente?.Nome ?? dto?.NomePaciente ?? string.Empty;
        var cpf = paciente?.CPF ?? dto?.Cpf ?? string.Empty;
        var idade = paciente != null
            ? DateTime.Today.Year - paciente.Nascimento.Year
            : dto?.Idade ?? 0;

        return new DescricaoBasica(
            nome,
            cpf,
            idade,
            dto?.Profissao ?? string.Empty,
            dto?.Religiao ?? string.Empty,
            dto?.QD ?? string.Empty,
            dto?.AtividadeFisica);
    }

    private static AGO MapDtoToAGO(AGODto? dto)
    {
        if (dto == null)
            return new AGO("", "", "", "", StatusVacinaHPV.SemInfo, "", "");

        return new AGO(
            dto.Menarca,
            dto.DUM,
            dto.Paridade,
            dto.DesejoGestacao,
            dto.VacinaHPV,
            dto.CCO,
            dto.MAC_TRH,
            dto.Intercorrencias,
            dto.Amamentacao,
            dto.VidaSexual,
            dto.Relacionamento,
            dto.Parceiros,
            dto.Coitarca,
            dto.IST);
    }

    private static Antecedentes MapDtoToAntecedentes(AntecedentesDto? dto)
    {
        if (dto == null)
            return new Antecedentes("", "", "", "", "", "");

        return new Antecedentes(
            dto.Comorbidades,
            dto.Medicacao,
            dto.Neoplasias,
            dto.Cirurgias,
            dto.Alergias,
            dto.Vicios,
            dto.HabitoIntestinal,
            dto.Vacinas);
    }

    private static AntecedentesFamiliares MapDtoToAntecedentesFamiliares(AntecedentesFamiliaresDto? dto)
    {
        if (dto == null) return new AntecedentesFamiliares("", "");
        return new AntecedentesFamiliares(dto.Neoplasias, dto.Comorbidades);
    }

    private static PosOp? MapDtoToPosOp(PosOpDto? dto)
    {
        if (dto == null) return null;
        return new PosOp(dto.PeriodoSeguimento, dto.Conclusao, dto.ExameMacro);
    }

    private static List<ProntuarioAcaoCD>? MapDtoToAcoesCD(List<AcoesCDDto>? dtos)
    {
        if (dtos == null) return null;
        return dtos.Select(d => new ProntuarioAcaoCD(Guid.Empty, d.Tipo)).ToList();
    }

    private static List<Exame>? MapDtoToExames(List<ExameDto>? dtos)
    {
        if (dtos == null) return null;
        return dtos.Select(d => new Exame(
            Guid.Empty, d.Codigo, d.Nome,
            d.Status, d.DataSolicitacao, d.DataResultado)).ToList();
    }

    private static Internacao? MapDtoToInternacao(SolicitacaoInternacaoDto? dto)
    {
        if (dto == null) return null;

        var internacao = new Internacao(
            Guid.Empty,
            dto.Data,
            dto.IndicacaoClinica,
            dto.Observacao,
            dto.CIDCodigo,
            dto.TempoDoenca,
            dto.Diarias,
            dto.Tipo,
            dto.Regime,
            dto.Carater,
            dto.UsaOPME,
            dto.Local,
            dto.Guia);

        if (dto.Procedimentos != null)
        {
            foreach (var procDto in dto.Procedimentos)
            {
                internacao.Procedimentos.Add(
                    new ProcedimentoInternacao(internacao.ID, procDto.CodigoProcedimento, procDto.Descricao));
            }
        }

        return internacao;
    }
}