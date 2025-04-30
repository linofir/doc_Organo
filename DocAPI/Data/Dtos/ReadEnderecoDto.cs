using System.ComponentModel.DataAnnotations;
using DocAPI.Core.Models;

namespace DocAPI.Data.Dtos;

public class ReadEnderecoDto
{
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string  Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
}