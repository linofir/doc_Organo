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
public class ProntuarioController : ControllerBase
{
    //private PacienteContext _context;
    private readonly IProntuarioRepository _repository;
    private IMapper _mapper;

    public ProntuarioController(IProntuarioRepository repository, IMapper mapper)
    {
        //_context = context;
        _repository = repository;
        _mapper = mapper;
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateProntuarioDto dto)
    {
        var prontuario = _mapper.Map<Prontuario>(dto);

        await _repository.CreateAsync(prontuario);
        Console.WriteLine($"O prontuário d@ {prontuario.DescricaoBasica.NomePaciente} foi efetuado ");
        Console.WriteLine($"foi criado o ID: {prontuario.ID}");
        var prontuarioDto = _mapper.Map<ReadProntuarioDto>(prontuario);
        return CreatedAtAction(nameof(GetByID), new { id = prontuario.ID }, prontuarioDto);
        // return CreatedAtAction(nameof(GetByID), new { id = prontuario.ID }, prontuario);
    }
    [HttpPost("from-pdf")] // Use uma rota mais descritiva
    public async Task<IActionResult> PostFromPDF([FromForm] string pacienteId, [FromForm] IFormFile pdfFile)
    {
        if (pdfFile == null || pdfFile.Length == 0)
        {
            return BadRequest("Nenhum arquivo PDF foi enviado.");
        }

        if (string.IsNullOrWhiteSpace(pacienteId))
        {
            return BadRequest("O ID do paciente é obrigatório.");
        }

        // Aqui você precisará de um caminho temporário ou de um Stream para passar para o repositório
        // Geralmente, para processamento imediato, um MemoryStream é uma boa opção.
        // Se o seu repositório espera um 'pdfPath' (caminho físico), você precisará salvar o arquivo temporariamente.
        string tempFilePath = null;
        Prontuario prontuario = null;

        try
        {
            // Opção A: Passar um Stream diretamente (melhor se seu repositório puder lidar com Stream)
            // using (var stream = pdfFile.OpenReadStream())
            // {
            //     prontuario = await _repository.CreateFromPdfStreamAsync(pacienteId, stream);
            // }

            // Opção B: Salvar temporariamente para passar o caminho (se seu repositório espera um path)
            // Certifique-se de ter uma forma de gerar um caminho temporário seguro
            tempFilePath = Path.GetTempFileName();
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await pdfFile.CopyToAsync(stream);
            }

            prontuario = await _repository.CreateFromPdfAsync(pacienteId, tempFilePath);

            if (prontuario == null)
            {
                return StatusCode(500, "Erro ao processar o PDF e criar o prontuário.");
            }

            Console.WriteLine($"O prontuário d@ {prontuario.DescricaoBasica.NomePaciente} foi efetuado ");
            Console.WriteLine($"foi criado o ID: {prontuario.ID}");

            // Mapeie para um DTO de leitura se necessário
            var prontuarioDto = _mapper.Map<ReadProntuarioDto>(prontuario);

            // Retorna 201 Created
            return CreatedAtAction(nameof(GetByID), new { id = prontuario.ID }, prontuarioDto);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Erro ao processar PDF: {ex.Message}");
            return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
        }
        finally
        {
            // Garante que o arquivo temporário seja excluído, se foi criado
            if (tempFilePath != null && System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Delete(tempFilePath);
            }
        }
    }
    [HttpGet]
    public async Task<IActionResult> GetProntuarios([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        if(_repository == null) return NotFound();
        var prontuarios = await _repository.GetAllAsync(skip, take);
        return Ok(_mapper.Map<IEnumerable<ReadProntuarioDto>>(prontuarios));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID(string id)
    {
        var prontuario = await _repository.GetByIdAsync(id);
        if (prontuario == null) return NotFound();
        return Ok(_mapper.Map<ReadProntuarioDto>(prontuario));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProntuario(string id, [FromBody] UpdateProntuarioDto dto)
    {
        try
        {
            // 1. Verifica se o ID foi fornecido
            if (string.IsNullOrEmpty(id))
                return BadRequest("O ID do prontuario é obrigatório.");
            if (dto == null)
                return BadRequest("O corpo da requisição está vazio ou inválido.");
            // 2. Mapeia o DTO para a entidade Paciente
            var prontuarioAtualizado = _mapper.Map<Prontuario>(dto);

            // 3. Atualiza o paciente na planilha
            await _repository.UpdateAsync(prontuarioAtualizado, id);

            // 4. Retorna sucesso
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar prontuario: {ex.Message}");
            return StatusCode(500, "Erro interno ao atualizar prontuario.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProntuario(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            return BadRequest("O ID do prontuario é obrigatório.");
            await _repository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao excluir prontuario: {ex.Message}");
            return NotFound("Prontuario não encontrado.");
        }
    }
    



}