using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DocAPI.Core.Models;

public class Endereco
{
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string  Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
}