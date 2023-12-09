using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DocAPI.Models;

public class Consultorio
{
    [Key]
    [Required(ErrorMessage =" Um ID é necessário")]
    public string? ID { get; set; }
    [Required(ErrorMessage ="O endereço é necessário")]
    public string? Logradouro { get; set; }
    [Required(ErrorMessage ="O número é necessário")]
    public int Numero { get; set; }
    public string? Complemento { get; set; }
    public virtual ICollection<Consulta>? Consultas { get; set; }
    public Consultorio()
    {
        ID = Guid.NewGuid().ToString();
    }
}