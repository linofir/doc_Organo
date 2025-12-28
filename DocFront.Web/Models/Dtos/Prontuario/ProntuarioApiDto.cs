namespace DocFront.Models.Dtos;
public class ProntuarioApiDto
{
    public DateOnly? DataConsulta {get; set; } 
    public string? Tipo {get; set; }
    public DescricaoBasicaDto? DescricaoBasica { get; set; }
    public AGODto? AGO { get; set; }
    public AntecedentesDto? Antecedentes { get; set; }
    public AntecedentesFamiliaresDto? AntecedentesFamiliares { get; set; }
    public List<AcoesCD>? CD { get; set; }   
    public string? InformacoesExtras { get; set; } = string.Empty;
    public List<ExameDto>? Exames { get; set; }
    public InternacaoDto? SolicitacaoInternacao { get; set;}
    public PosOpDto? PosOperatorio  { get; set;}
}
