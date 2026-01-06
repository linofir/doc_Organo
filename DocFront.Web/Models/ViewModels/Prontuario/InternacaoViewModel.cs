namespace DocFront.Models.ViewModels;

public class InternacaoViewModel
{
    public List<string> Procedimentos { get; set; } = new();
    public DateOnly Data { get; set; }
    public string? IndicacaoClinica { get; set; } 
    public string? Observacao { get; set; } 
    public string? CID { get; set; }  
    public string? TempoDoenca { get; set; } 
    public string? Diarias { get; set; } 
    public string? Tipo { get; set; } 
    public string? Regime { get; set; } 
    public string? Carater { get; set; } 
    public bool UsaOPME { get; set; } = false;
    public string? Local  { get; set; }  
    public string? Guia  { get; set; }
}
