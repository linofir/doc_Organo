namespace DocFront.Models.Dtos;
using DocFront.Models.Enums;

public class ReadProntuarioDto
{
    public string Id { get; set; } = string.Empty;
    public DateOnly DataConsulta { get; set; } 
    public string? Tipo { get; set; }

    public DescricaoBasicaDto? DescricaoBasica { get; set; }
    public AGODto? Ago { get; set; }
    public AntecedentesDto? Antecedentes { get; set; }
    public AntecedentesFamiliaresDto? AntecedentesFamiliares { get; set; }

    public List<AcoesCd>? Cd { get; set; }
    public string? InformacoesExtras { get; set; }

    public List<ExameDto>? Exames { get; set; }
    public InternacaoDto? SolicitacaoInternacao { get; set; }
    public PosOpDto? PosOperatorio { get; set; }
}
