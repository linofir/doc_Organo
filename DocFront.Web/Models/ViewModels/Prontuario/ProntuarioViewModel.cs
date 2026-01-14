using DocFront.Models.Enums;
using DocFront.Models.ViewModels;

namespace DocFront.Models.ViewModels;

public class ProntuarioViewModel
{
    public string? Id { get; set; }

    public DateOnly? DataConsulta { get; set; } // 🔑 DateTime para InputDate

    public string? Tipo { get; set; }

    public DescricaoBasicaViewModel DescricaoBasica { get; set; } = new();

    public AGOViewModel AGO { get; set; } = new();

    public AntecedentesViewModel Antecedentes { get; set; } = new();

    public AntecedentesFamiliaresViewModel AntecedentesFamiliares { get; set; } = new();

    public List<AcoesCd> AcoesCD { get; set; } = new();

    public string? InformacoesExtras { get; set; }

    public List<ExameViewModel> Exames { get; set; } = new();

    public InternacaoViewModel? SolicitacaoInternacao { get; set; }

    public PosOpViewModel? PosOperatorio { get; set; }

    // 🔹 Estados de UI (não existem na API!)
    public bool IsEditando { get; set; }
    public bool IsValido { get; set; }
}
