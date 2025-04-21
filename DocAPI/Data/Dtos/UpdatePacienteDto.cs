using System.ComponentModel.DataAnnotations;
using DocAPI.Models;

namespace DocAPI.Data.Dtos;

public class UpdatePacienteDto
{
    [Required(ErrorMessage = "O nome do paciente é obrigatório")]
    public string Nome { get; set; }
    [Required(ErrorMessage = "O Nascimento do paciente é obrigatório")] 
    public DateTime Nascimento { get; set; }
    [Required(ErrorMessage = "O CPF do paciente é obrigatório")]
    [StringLength(11, ErrorMessage = "O máximo de caracteres é 11")]
    public string CPF { get; set; }
    [StringLength(11, ErrorMessage = "O máximo de caracteres é 11")]
    [MaxLength(11)]
    public string? RG { get; set; }
    [Required(ErrorMessage = "O email do paciente é obrigatório")]
    public string Email { get; set; }
    [Required(ErrorMessage = "O telefone do paciente é obrigatório")]
    public string Telefone { get; set; }
    [Required(ErrorMessage = "O Plano do paciente é obrigatório")]
    public string Plano { get; set; }
    [Required(ErrorMessage = "O Carteira do paciente é obrigatório")]
    public string Carteira { get; set; }
    public UpdateEnderecoDto Endereco { get; set; } = new();
    
    
}