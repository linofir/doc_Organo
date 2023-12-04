using System.ComponentModel.DataAnnotations;

namespace DocAPI.Data.Dtos;

public class CreateConsultorioDto
{
    [Required(ErrorMessage ="O endereçõ é necessário")]
    public string? Logradouro { get; set; }
    [Required(ErrorMessage ="O número é necessário")]
    public int Numero { get; set; }
    public string? Complemento { get; set; }

}