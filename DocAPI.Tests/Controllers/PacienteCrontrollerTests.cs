using System.Data.Common;
using AutoMapper;
using DocAPI.Controllers;
using DocAPI.Core.Entities;
using DocAPI.Core.Interfaces.Repositories;
using DocAPI.Data.Dtos;
using DocAPI.Infrastructure.Repositories;
using DocAPI.Interfaces.Repositories;
using DocAPI.Profiles;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Swashbuckle.AspNetCore.SwaggerUI;
using Xunit;


namespace DocAPI.Tests.Controllers;

public class PacienteControllerTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<PacienteProfile>());
        return config.CreateMapper();
    }

    [Fact]
    public async Task Post_ValidDto_ReturnsCreated()
    {
        var createDto = new CreatePacienteDto {
            Nome = "PaceinteController",
            Nascimento = new DateOnly(1990, 1, 1),
            CPF = "12345678901",
            RG = "117774475",
            Email = "testeController@ex.com",
            Telefone = "11988887777",
            Plano = "PlanoController",
            Carteira = "741852963",
            Endereco = new CreateEnderecoDto
            {
                Logradouro =" Rua Teste",
                Numero = "123",
                Bairro = "Controller",
                Cidade = "ControllerTest",
                UF = "TT",
                CEP = "01122033"
            }
        };
       
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller  = new PacienteController(pacienteRepo.Object, CreateMapper());
        pacienteRepo.Setup(r => r.CreateAsync(It.IsAny<Paciente>())).Returns(Task.CompletedTask);

        var result = await controller.Post(createDto);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var dto = Assert.IsType<ReadPacienteDto>(created.Value);
        Assert.Equal(createDto.Nome, dto.Nome);
        Assert.Equal(createDto.Nascimento, dto.Nascimento);
        Assert.Equal(createDto.CPF, dto.CPF);
        Assert.Equal(createDto.RG, dto.RG);
        Assert.Equal(createDto.Email, dto.Email);
        Assert.Equal(createDto.Telefone, dto.Telefone);  
        Assert.Equal(createDto.Plano, dto.Plano);  
        Assert.Equal(createDto.Carteira, dto.Carteira); 
        Assert.Equal(createDto.Endereco.Logradouro, dto.Endereco.Logradouro);
        Assert.Equal(createDto.Endereco.Numero, dto.Endereco.Numero); 
        Assert.Equal(createDto.Endereco.Bairro, dto.Endereco.Bairro);
        Assert.Equal(createDto.Endereco.Cidade, dto.Endereco.Cidade);
        Assert.Equal(createDto.Endereco.UF, dto.Endereco.UF);
        Assert.Equal(createDto.Endereco.CEP, dto.Endereco.CEP);
    }
    [Fact]
    public async Task Post_DuplicateCpfViolation_Returns409()
    {
        var createdDto = new CreatePacienteDto {
            Nome = "PaceinteController",
            Nascimento = new DateOnly(1990, 1, 1),
            CPF = "12345678901",
            RG = "117774475",
            Email = "testeController@ex.com",
            Telefone = "11988887777",
            Plano = "PlanoController",
            Carteira = "741852963",
            Endereco = new CreateEnderecoDto
            {
                Logradouro =" Rua Teste",
                Numero = "123",
                Bairro = "Controller",
                Cidade = "ControllerTest",
                UF = "TT",
                CEP = "01122033"
            }
        };
        var innerException = new Exception("Violation of UNIQUE KEY constraint");
        var exception = new DbUpdateException("Erro de banco", innerException);
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller  = new PacienteController(pacienteRepo.Object, CreateMapper());
        pacienteRepo.Setup(r => r.CreateAsync(It.IsAny<Paciente>())).Throws(exception);;

        var result = await controller.Post(createdDto);
        Assert.IsType<ConflictObjectResult>(result);
    }
    [Fact]
    public async Task Get_AllPacientes_Returns200()
    {
        
        var pacienteList = new List<Paciente> { 
            new Paciente("P1", new DateOnly(1990,1,1), "111", "e1@x.com", "99"),
            new Paciente("P2", new DateOnly(1990,1,1), "222", "e2@x.com", "99")
        };

        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller  = new PacienteController(pacienteRepo.Object, CreateMapper());
        pacienteRepo.Setup(r => r.GetAllAsync(0,100))
                    .ReturnsAsync(pacienteList);

        var result = await controller.GetPacientes();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<ReadPacienteDto>>(okResult.Value).ToList();
        Assert.Equal(2, dtos.Count);
        Assert.Contains(dtos, d => d.Nome == "P1"); 
        Assert.Contains(dtos, d => d.Nome == "P2");
    }
    [Fact]
    public async Task Get_InvalidPagingOnAllPacientes_Returns400()
    {
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller  = new PacienteController(pacienteRepo.Object, CreateMapper());
        pacienteRepo.Setup(r => r.GetAllAsync( -1, -100))
                    .ReturnsAsync(new List<Paciente>());
        
        var result = await controller.SearchByNome("", -1, -100);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task Get_ById_Returns200()
    {
        var paciente = new Paciente("P1", new DateOnly(1990,1,1), "111", "e1@x.com", "99");
        var pacienteId = paciente.ID;
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller = new PacienteController(pacienteRepo.Object, CreateMapper());
        pacienteRepo.Setup(r => r.GetByIdAsync(pacienteId)).ReturnsAsync(paciente);

        var result = await controller.GetByID(pacienteId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsAssignableFrom<ReadPacienteDto>(okResult.Value);
        Assert.Equal(dto.ID,  paciente.ID.ToString());
    }
    [Fact]
    public async Task Get_ById_Returns404()
    {
    
        var idInexistente = Guid.NewGuid();
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller  = new PacienteController(pacienteRepo.Object, CreateMapper());
        pacienteRepo.Setup(r => r.GetByIdAsync(idInexistente)).ReturnsAsync((Paciente?)null);

        var result = await controller.GetByID(idInexistente);
        Assert.IsType<NotFoundResult>(result);
    
    }
    [Fact]
    public async Task Get_ByCpf_Returns200()
    {
        var pacienteList = new List<Paciente> { 
            new Paciente("P1", new DateOnly(1990,1,1), "111", "e1@x.com", "99"),
        };
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller = new PacienteController(pacienteRepo.Object, CreateMapper());
        pacienteRepo.Setup(r => r.GetPacienteByCpfAsync(pacienteList[0].CPF)).ReturnsAsync(pacienteList);

        var result = await controller.GetByCpf(pacienteList[0].CPF);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ReadPacienteDto>(okResult.Value);
        Assert.Equal("111", dto.CPF);
        
    }
    [Fact]
    public async Task Get_EmpityCpfField_Returns400()
    {
        var pacienteList = new List<Paciente> { 
            new Paciente("P1", new DateOnly(1990,1,1), "111", "e1@x.com", "99"),
        };
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller = new PacienteController(pacienteRepo.Object, CreateMapper());

        var result = await controller.GetByCpf("");

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task Get_DuplicateCpfViolation_Returns409()
    {
        var pacienteList = new List<Paciente> { 
            new Paciente("P1", new DateOnly(1990,1,1), "111", "e1@x.com", "99"),
            new Paciente("P1", new DateOnly(1990,1,1), "111", "e1@x.com", "99")
        };
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller = new PacienteController(pacienteRepo.Object, CreateMapper());
        pacienteRepo.Setup(r => r.GetPacienteByCpfAsync(pacienteList[0].CPF)).ReturnsAsync(pacienteList);

        var result = await controller.GetByCpf(pacienteList[0].CPF);

        Assert.IsType<ConflictObjectResult>(result);
    }
    [Fact]
    public async Task Get_SearchByName_Returns200()
    {
        var pacienteList = new List<Paciente> { 
            new Paciente("P1", new DateOnly(1990,1,1), "111", "e1@x.com", "99"),
            new Paciente("P2", new DateOnly(1990,1,1), "222", "e2@x.com", "99")
        };

        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller  = new PacienteController(pacienteRepo.Object, CreateMapper());
        pacienteRepo.Setup(r => r.GetPacienteByNomeAsync("P1",0,100))
                    .ReturnsAsync(pacienteList);

        var result = await controller.SearchByNome("P1",0, 100);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<ReadPacienteDto>>(okResult.Value).ToList();
        Assert.Equal(2, pacienteList.Count());
        Assert.Equal("P1", dtos[0].Nome);
    }
    [Fact]
    public async Task SearchByNome_ShouldReturnEmptyList_WhenNoMatchFound()
    {
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller  = new PacienteController(pacienteRepo.Object, CreateMapper());

        pacienteRepo.Setup(r => r.GetPacienteByNomeAsync("Inexistente", 0, 100))
                    .ReturnsAsync(new List<Paciente>());

        
        var result = await controller.SearchByNome("Inexistente", 0, 100);

        
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<ReadPacienteDto>>(okResult.Value);
        Assert.Empty(dtos); 
    }
    [Fact]
    public async Task Get_InvalidPagingOnSearchByName_Returns400()
    {
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller  = new PacienteController(pacienteRepo.Object, CreateMapper());
        pacienteRepo.Setup(r => r.GetPacienteByNomeAsync("nome", -1, -100))
                    .ReturnsAsync(new List<Paciente>());
        
        var result = await controller.SearchByNome("", -1, -100);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task Get_EmpitySearchField_Returns400()
    {
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller  = new PacienteController(pacienteRepo.Object, CreateMapper());
        pacienteRepo.Setup(r => r.GetPacienteByNomeAsync("", 0, 100))
                    .ReturnsAsync(new List<Paciente>());
        
        var result = await controller.SearchByNome("", 0, 100);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task Put_ValidDto_Returns204()
    {
        var createDto = new UpdatePacienteDto {
            Nome = "PaceinteController",
            Nascimento = new DateOnly(1990, 1, 1),
            CPF = "12345678901",
            RG = "117774475",
            Email = "testeController@ex.com",
            Telefone = "11988887777",
            Plano = "PlanoController",
            Carteira = "741852963",
            Endereco = new UpdateEnderecoDto
            {
                Logradouro =" Rua Teste",
                Numero = "123",
                Bairro = "Controller",
                Cidade = "ControllerTest",
                UF = "TT",
                CEP = "01122033"
            }
        };
        var paciente = new Paciente(
            "Paciente Controller",
            new DateOnly(1990, 1, 1),
            "12345678901",
            "controller@example.com",
            "31990001111");
        var endereco = new Endereco
            {
                Logradouro =" Rua Teste",
                Numero = "123",
                Bairro = "Controller",
                Cidade = "ControllerTest",
                UF = "TT",
                CEP = "01122033"
            };
        paciente.ComplementarCadastro("117774475", "PlanoController", "741852963", endereco);
               
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller  = new PacienteController(pacienteRepo.Object, CreateMapper());

        pacienteRepo.Setup(r => r.UpdateAsync(It.IsAny<Paciente>(), paciente.ID)).Returns(Task.CompletedTask);

        var result = await controller.UpdatePaciente(paciente.ID, createDto);
        Assert.IsType<NoContentResult>(result);
        
    }
    [Fact]
    public async Task Put_DuplicateCpfViolation_Returns409()
    {
        var createDto = new UpdatePacienteDto {
            Nome = "PaceinteController",
            Nascimento = new DateOnly(1990, 1, 1),
            CPF = "12345678901",
            RG = "117774475",
            Email = "testeController@ex.com",
            Telefone = "11988887777",
            Plano = "PlanoController",
            Carteira = "741852963",
            Endereco = new UpdateEnderecoDto
            {
                Logradouro =" Rua Teste",
                Numero = "123",
                Bairro = "Controller",
                Cidade = "ControllerTest",
                UF = "TT",
                CEP = "01122033"
            }
        };
        var paciente = new Paciente(
            "Paciente Controller",
            new DateOnly(1990, 1, 1),
            "12345678901",
            "controller@example.com",
            "31990001111");
        var endereco = new Endereco
            {
                Logradouro =" Rua Teste",
                Numero = "123",
                Bairro = "Controller",
                Cidade = "ControllerTest",
                UF = "TT",
                CEP = "01122033"
            };
        paciente.ComplementarCadastro("117774475", "PlanoController", "741852963", endereco);
               
        var innerException = new Exception("Violation of UNIQUE KEY constraint");
        var exception = new DbUpdateException("Erro de banco", innerException);
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller  = new PacienteController(pacienteRepo.Object, CreateMapper());

        pacienteRepo.Setup(r => r.UpdateAsync(It.IsAny<Paciente>(), paciente.ID)).Throws(exception);

        var result = await controller.UpdatePaciente(paciente.ID, createDto);
        Assert.IsType<ConflictObjectResult>(result);
        
    }
    [Fact]
    public async Task Delete_IdExists_Returns204()
    {
        var paciente = new Paciente("P1", new DateOnly(1990,1,1), "111", "e1@x.com", "99");
               
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller  = new PacienteController(pacienteRepo.Object, CreateMapper());

        pacienteRepo.Setup(r => r.DeleteAsync(paciente.ID)).Returns(Task.CompletedTask);

        var result = await controller.DeletePaciente(paciente.ID);
        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task Delete_Returns404_WhenIdDoesNotExist()
    {
        var idInexistente = Guid.NewGuid();
        var pacienteRepo = new Mock<IPacienteRepository>();
        var controller  = new PacienteController(pacienteRepo.Object, CreateMapper());
        pacienteRepo.Setup(r => r.DeleteAsync(idInexistente)).Throws(new KeyNotFoundException());

        var result = await controller.DeletePaciente(idInexistente);
        Assert.IsType<NotFoundResult>(result); 
    }

}
