// using Microsoft.AspNetCore.Components;
// using DocFront.Models;

// public partial class GerenciarPacientes : ComponentBase
// {
//     private string BuscaTermo = string.Empty;
//     private bool Buscou = false;

//     private List<PacienteModel> Pacientes = new();
//     private string? PacienteSelecionadoId;

//     protected async Task BuscarPacientes()
//     {
//         Buscou = true;

//         if (string.IsNullOrWhiteSpace(BuscaTermo))
//         {
//             Pacientes = new();
//             return;
//         }

//         Pacientes = await PacienteService.GetAll(BuscaTermo);
//         PacienteSelecionadoId = null;
//     }

//     private void AbrirPaciente()
//     {
//         if (PacienteSelecionadoId == null) return;
//         NavigationManager.NavigateTo($"/pacientes/detalhes/{PacienteSelecionadoId}");
//     }

//     private void EditarPaciente()
//     {
//         if (PacienteSelecionadoId == null) return;
//         NavigationManager.NavigateTo($"/pacientes/editar/{PacienteSelecionadoId}");
//     }

//     private async Task DeletarPaciente()
//     {
//         if (PacienteSelecionadoId == null) return;

//         await PacienteService.Delete(PacienteSelecionadoId);
//         await BuscarPacientes(); // Atualiza lista após deletar
//     }
// }
