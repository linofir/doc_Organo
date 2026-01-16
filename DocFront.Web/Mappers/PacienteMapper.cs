using DocFront.Models;
using DocFront.Models.Dtos;

namespace DocFront.Mappers;
public static class PacienteMapper
{
    //POST e PUT
    public static PacienteApiDto ToApi(PacienteModel model)
    {
        return new PacienteApiDto
        {
            Nome = model.Nome,
            CPF = model.CPF,
            Nascimento = model.Nascimento,
            Email = model.Email,
            Telefone = model.Telefone,
            Plano = model.Plano,
            Carteira = model.Carteira,
            RG = model.RG,

            Endereco = new EnderecoDto
            {
                Logradouro = model.Logradouro,
                Numero = model.Numero,
                Bairro = model.Bairro,
                Cidade = model.Cidade,
                UF = model.UF,
                CEP = model.CEP
            }
        };
    }
    //GET
    public static PacienteModel ToModel(string id, PacienteApiDto api)
    {
        if (api == null) return new PacienteModel();
        return new PacienteModel
        {
            ID = id,
            Nome = api.Nome,
            CPF = api.CPF,
            RG = api.RG,
            Email = api.Email,
            Telefone = api.Telefone,
            Plano = api.Plano,
            Carteira = api.Carteira,
            Nascimento = api.Nascimento,

            // 🔽 flatten do endereço
            Logradouro = api.Endereco.Logradouro,
            Numero = api.Endereco.Numero,
            Bairro = api.Endereco.Bairro,
            Cidade = api.Endereco.Cidade,
            UF = api.Endereco.UF,
            CEP = api.Endereco.CEP
        }; 
        
    }
    public static List<PacienteListDto> ToCard(List<PacienteModel> model)
    {
        if (model is null) return new List<PacienteListDto>();
        var listDtos = model.Select(p => new PacienteListDto
        {
            Id = p.ID,
            Nome = p.Nome,
            CPF = p.CPF,
            Email = p.Email
        }).ToList();
        return listDtos;
        
    }
    

    
}