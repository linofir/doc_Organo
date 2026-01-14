using System.Text.Json.Serialization;

namespace DocFront.Models.Dtos;

public class InternacaoDto
{
    public List<string> Procedimentos { get; set; } = new();
    public DateOnly Data { get; set; }
    public string IndicacaoClinica { get; set; } = string.Empty; 
    public string Observacao { get; set; } = string.Empty; 
    public string CID { get; set; } = string.Empty;  
    public string TempoDoenca { get; set; } = string.Empty; 
    public string Diarias { get; set; } = string.Empty; 
    public string Tipo { get; set; } = string.Empty; 
    public string Regime { get; set; } = string.Empty; 
    public string Carater { get; set; } = string.Empty; 
    [JsonPropertyName("usaOPME")]
    public bool UsaOPME { get; set; } = false;
    public string Local  { get; set; } = string.Empty;  
    public string Guia  { get; set; } = string.Empty;
}