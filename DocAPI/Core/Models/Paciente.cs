using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.ConstrainedExecution;
using System.Text.Json.Serialization;

namespace DocAPI.Core.Models;

public class Paciente
{
    public Paciente(){}
    // public Paciente()
    // {
    //     ID = Guid.NewGuid().ToString();
    // }
    [Key]
    [Required(ErrorMessage = "Este campo é obrigatório")]
    public string? ID { get; set; } = string.Empty;
    [Required(ErrorMessage = "O nome da paciente é obrigatório")]
    public string? Nome { get; set; }
    [Required(ErrorMessage = "O Nascimento da paciente é obrigatório")]
    public DateOnly Nascimento { get; set; }
    [NotMapped] 
    public int Idade
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var idade = today.Year - Nascimento.Year;
            if (Nascimento > today.AddYears(-idade)) idade--;
            return idade;
        }
    }
    [Required(ErrorMessage = "O CPF da paciente é obrigatório")]
    [MaxLength(11, ErrorMessage = "O máximo de caracteres é 11")]
    public string? CPF { get; set; }
    [MaxLength(11, ErrorMessage = "O máximo de caracteres é 11")]
    public string? RG { get; set; }
    [Required(ErrorMessage = "O email da paciente é obrigatório")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "O telefone da paciente é obrigatório")]
    public string? Telefone { get; set; }
    [Required(ErrorMessage = "O Plano de saúde do paciente é obrigatório")]
    public string? Plano { get; set; }// possível enum
    [Required(ErrorMessage = "A carteira da paciente é obrigatório")]
    public string? Carteira { get; set; }
    public Endereco? Endereco { get; set; } 
    public string? Descricao 
    {
        get
        {
            string descricao = $@"
        ID = {ID}
        Nome = {Nome}
        Nascimento = {Nascimento:dd/MM/yyyy}
        Idade = {Idade}
        CPF = {CPF}
        RG = {RG}
        Email = {Email}
        Telefone = {Telefone}
        Plano = {Plano}
        Carteira = {Carteira}
        Endereço = Rua/Av: {Endereco?.Logradouro}, {Endereco?.Numero}, {Endereco?.Bairro}, {Endereco?.Cidade}, {Endereco?.UF}, {Endereco?.CEP}

        ";
            return descricao;
        }
    }
}