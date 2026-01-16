namespace DocFront.Models.Dtos;
public class SenhaDto
{
    public string Codigo { get; set; } = string.Empty;
    public DateOnly DataPedido { get; set; }
    public DateOnly? DataLibetracao { get; set; }
    public DateOnly? Validade { get; set; }
}   