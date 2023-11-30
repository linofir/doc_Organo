using System.ComponentModel.DataAnnotations;

namespace DocAPI.Data.Dtos;

public class UpdatePacienteDto
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
    
}