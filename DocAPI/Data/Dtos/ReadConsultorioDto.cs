
using System.Text.Json.Serialization;

namespace DocAPI.Data.Dtos;

public class ReadConsultorioDto
{
    public string? Logradouro { get; set; }
    public int Numero { get; set; }
    public string? Complemento { get; set; }
    //public ICollection<ReadConsultaDto>? Consultas { get; set; }

}