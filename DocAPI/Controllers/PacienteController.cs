using DocAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocAPI.Controllers;

[ApiController]
[Route("[Controller]")]
public class PacienteController : ControllerBase
{
    [HttpPost]
    private static void CadastrarPaciente([FromBody] Paciente paciente)
    {
        Pacientes NovaLista = new();
        NovaLista.AdicionarNovoPaciente(paciente);
        Console.WriteLine($"O cadastro d@ {paciente.Nome} foi efetuado ");
        
    }
}