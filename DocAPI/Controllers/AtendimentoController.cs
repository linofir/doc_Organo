using AutoMapper;
using DocAPI.Data;
using DocAPI.Data.Dtos.ProntuarioDtos;
using DocAPI.Core.Models;
using DocAPI.Core.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AtendimentoController : ControllerBase
{
    //private PacienteContext _context;
    private readonly IAtendimentoRepository _repository;
    private IMapper _mapper;

    public AtendimentoController(IAtendimentoRepository repository, IMapper mapper)
    {
        //_context = context;
        _repository = repository;
        _mapper = mapper;
    }
    [HttpGet("report-id/{id}")]
    public async Task<IActionResult> GetPatientReportPdf(string id)
    {
        // 1. Validação de entrada (Ex: se o ID não é vazio)
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("O ID do paciente não pode ser vazio.");
        }

        try
        {
            // 2. Chama o repositório que contém a lógica de negócio e as validações
            var pdfStream = await _repository.CreateReportByIdAsync(id);

            // 3. Retorna o resultado (se nenhuma exceção foi lançada)
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            return File(pdfStream, "application/pdf", $"RelatorioPaciente_{id}.pdf");
        }
        catch (InvalidOperationException ex) // Captura a exceção de negócio
        {
            return NotFound(ex.Message); // Retorna 404 Not Found
        }
        catch (Exception ex)
        {
            // Logar o erro completo para depuração (ex: via ILogger)
            Console.Error.WriteLine($"Erro inesperado ao gerar relatório PDF para paciente ID {id}: {ex.Message} - {ex.StackTrace}");
            return StatusCode(500, "Erro interno do servidor ao gerar o relatório."); // Retorna 500 Internal Server Error
        }
    }
}