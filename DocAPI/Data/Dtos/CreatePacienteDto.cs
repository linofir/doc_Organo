using System.ComponentModel.DataAnnotations;

namespace DocAPI.Data.Dtos;

public class CreatePacienteDto
{
    [Required(ErrorMessage = "O nome do paciente é obrigatório")]
    public string? Nome { get; set; }
    [Required(ErrorMessage = "O Nascimento do paciente é obrigatório")]
    public string? Nascimento { get; set; }
    public int Idade { get; set; }
    [Required(ErrorMessage = "O CPF do paciente é obrigatório")]
    [StringLength(11, ErrorMessage = "O máximo de caracteres é 11")]
    public string? CPF { get; set; }
    [Required(ErrorMessage = "O RG do paciente é obrigatório")]
    [StringLength(11, ErrorMessage = "O máximo de caracteres é 11")]
    public string? RG { get; set; }
    [Required(ErrorMessage = "O email do paciente é obrigatório")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "O telefone do paciente é obrigatório")]
    public string? Telefone { get; set; }
    
    
    // public Paciente(string id, string? nome, string nascimento, int idade, string cpf, string rg, string? email, string? telefone, int myProperty)
    // {
        // ID = id ?? Guid.NewGuid().ToString();
        // Nome = nome;
        // Nascimento = nascimento;
        // Idade = idade;
        // CPF = cpf;
        // RG = rg;
        // Email = email;
        // Telefone = telefone;

    // }
    // public Paciente()
    // {
    //     ID = Guid.NewGuid().ToString();
    // }
}