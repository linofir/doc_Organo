using DocFront.Models.Enums;

namespace DocFront.Models.Dtos;
public class AGODto
{
    public string Menarca { get; set; } = string.Empty;
    public string DUM { get; set; } = string.Empty;
    public string Paridade { get; set; } = string.Empty;
    public string DesejoGestacao { get; set; } = string.Empty;
    public StatusVacinaHPV? VacinaHPV { get; set; } 
    public string CCO { get; set; } = string.Empty;
    public string MAC_TRH { get; set; } = string.Empty;

    public string Intercorrencias { get; set; } = string.Empty;
    public string Amamentacao { get; set; } = string.Empty;
    public string VidaSexual { get; set; } = string.Empty;
    public string Relacionamento { get; set; } = string.Empty;
    public string Parceiros { get; set; } = string.Empty;
    public string Coitarca { get; set; } = string.Empty;
    public string IST { get; set; } = string.Empty;
}
