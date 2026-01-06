namespace DocFront.Models.Dtos;

public class ProntuarioCardDto
{
    public string Id { get; set; } = "";
    public string Nome { get; set; } = "";
    public DateOnly? Data { get; set; } 
    public string Tipo { get; set; } = "";
    
}