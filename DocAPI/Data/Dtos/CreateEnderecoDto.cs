using System.ComponentModel.DataAnnotations;
using DocAPI.Models;

namespace DocAPI.Data.Dtos;

public class CreateEnderecoDto
{
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string  Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
}