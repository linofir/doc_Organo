namespace DocFront.Models.Dtos;

public class PacienteApiDto
{
    public string ID { get; set; } = "";
    public string CPF { get; set; } = "";
    public string Nome { get; set; } = "";
    public DateOnly Nascimento { get; set; }
    public string Plano { get; set; } = "";
    public string Carteira { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefone { get; set; } = "";
    public string RG { get; set; } = "";

    public EnderecoDto Endereco { get; set; } = new();
}

