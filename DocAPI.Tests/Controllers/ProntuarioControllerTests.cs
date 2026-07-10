using AutoMapper;
using DocAPI.Controllers;
using DocAPI.Core.Entities;
using DocAPI.Core.Interfaces.Repositories;
using DocAPI.Data.Dtos.ProntuarioDtos;
using DocAPI.Infrastructure.SqlDb.Context;
using DocAPI.Interfaces.Repositories;
using DocAPI.Profiles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DocAPI.Tests.Controllers;

public class ProntuarioControllerTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ProntuarioProfile>());
        return config.CreateMapper();
    }

    private static DocDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<DocDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new DocDbContext(options);
    }

    private static Paciente SamplePaciente()
    {
        return new Paciente(
            "Paciente Controller",
            new DateOnly(1990, 1, 1),
            "12345678901",
            "controller@test.com",
            "31990001111");
    }

    private static Atendimento SampleAtendimento(Guid pacienteId)
    {
        return new Atendimento(pacienteId, "Mensagem controller");
    }

    private static Prontuario SampleProntuario(Guid pacienteId, Guid atendimentoId)
    {
        var p = new Prontuario(pacienteId, atendimentoId);
        p.AplicarCriacao(
            new DateOnly(2026, 6, 1), 0, null,
            new DescricaoBasica("Joana", "12345678901", 36, "Eng", "Cat", "QD", null),
            new AGO("", "", "", "", StatusVacinaHPV.SemInfo, "", ""),
            new Antecedentes("", "", "", "", "", ""),
            new AntecedentesFamiliares("", ""),
            null, null, null, null);
        return p;
    }

    private static CreateProntuarioDto SampleCreateDto(Guid pacienteId, Guid atendimentoId)
    {
        return new CreateProntuarioDto
        {
            PacienteId = pacienteId,
            AtendimentoId = atendimentoId,
            DataConsulta = new DateOnly(2026, 6, 1),
            Tipo = 0,
            DescricaoBasica = new DescricaoBasicaDto
            {
                NomePaciente = "Joana",
                Cpf = "12345678901",
                Idade = 36,
                Profissao = "Eng",
                Religiao = "Cat",
                QD = "Dor",
                AtividadeFisica = "Corrida"
            },
            AGO = new AGODto
            {
                Menarca = "12",
                DUM = "01/06",
                Paridade = "G1",
                DesejoGestacao = "Sim",
                VacinaHPV = StatusVacinaHPV.SemInfo,
                CCO = "Normal",
                MAC_TRH = "Nenhum"
            },
            Antecedentes = new AntecedentesDto
            {
                Comorbidades = "Nenhuma",
                Medicacao = "Nenhuma",
                Neoplasias = "Nenhuma",
                Cirurgias = "Nenhuma",
                Alergias = "Nenhuma",
                Vicios = "Nenhum"
            },
            AntecedentesFamiliares = new AntecedentesFamiliaresDto
            {
                Neoplasias = "Nenhuma",
                Comorbidades = "Nenhuma"
            }
        };
    }

    // ═══════════════════════════════════════════════════════════
    //  POST /
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task Post_ValidDto_ReturnsCreated()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);

        var pacienteRepo = new Mock<IPacienteRepository>();
        pacienteRepo.Setup(r => r.GetByIdAsync(paciente.ID)).ReturnsAsync(paciente);

        var atendimentoRepo = new Mock<IAtendimentoRepository>();
        atendimentoRepo.Setup(r => r.GetByIdAsync(atendimento.Id)).ReturnsAsync(atendimento);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.AddAsync(It.IsAny<Prontuario>())).Returns(Task.CompletedTask);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object, pacienteRepo.Object, atendimentoRepo.Object,
            context, CreateMapper());

        var dto = SampleCreateDto(paciente.ID, atendimento.Id);
        var result = await controller.PostProntuario(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var readDto = Assert.IsType<ReadProntuarioDto>(created.Value);
        Assert.Equal(paciente.ID, readDto.PacienteId);
        Assert.Equal(atendimento.Id, readDto.AtendimentoId);
        Assert.Equal(1, readDto.Versao);
        Assert.Equal(0, readDto.Tipo);
        Assert.Equal(paciente.Nome, readDto.DescricaoBasica!.NomePaciente);
    }

    [Fact]
    public async Task Post_InvalidAtendimentoId_ReturnsNotFound()
    {
        var pacienteId = Guid.NewGuid();
        var atendimentoId = Guid.NewGuid();

        var pacienteRepo = new Mock<IPacienteRepository>();
        pacienteRepo.Setup(r => r.GetByIdAsync(pacienteId)).ReturnsAsync(SamplePaciente());

        var atendimentoRepo = new Mock<IAtendimentoRepository>();
        atendimentoRepo.Setup(r => r.GetByIdAsync(atendimentoId)).ReturnsAsync((Atendimento?)null);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object, pacienteRepo.Object, atendimentoRepo.Object,
            context, CreateMapper());

        var dto = SampleCreateDto(pacienteId, atendimentoId);
        var result = await controller.PostProntuario(dto);

        Assert.IsType<NotFoundResult>(result);
        prontuarioRepo.Verify(r => r.AddAsync(It.IsAny<Prontuario>()), Times.Never);
    }

    [Fact]
    public async Task Post_PacienteIdMismatch_ReturnsNotFound()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(Guid.NewGuid()); // Different PacienteId

        var pacienteRepo = new Mock<IPacienteRepository>();
        pacienteRepo.Setup(r => r.GetByIdAsync(paciente.ID)).ReturnsAsync(paciente);

        var atendimentoRepo = new Mock<IAtendimentoRepository>();
        atendimentoRepo.Setup(r => r.GetByIdAsync(atendimento.Id)).ReturnsAsync(atendimento);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object, pacienteRepo.Object, atendimentoRepo.Object,
            context, CreateMapper());

        var dto = SampleCreateDto(paciente.ID, atendimento.Id);
        var result = await controller.PostProntuario(dto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Post_InternacaoWithUnknownCid_ReturnsBadRequest()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);

        var pacienteRepo = new Mock<IPacienteRepository>();
        pacienteRepo.Setup(r => r.GetByIdAsync(paciente.ID)).ReturnsAsync(paciente);

        var atendimentoRepo = new Mock<IAtendimentoRepository>();
        atendimentoRepo.Setup(r => r.GetByIdAsync(atendimento.Id)).ReturnsAsync(atendimento);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        await using var context = CreateInMemoryContext(); // No CID seeded
        var controller = new ProntuarioController(
            prontuarioRepo.Object, pacienteRepo.Object, atendimentoRepo.Object,
            context, CreateMapper());

        var dto = SampleCreateDto(paciente.ID, atendimento.Id);
        dto.SolicitacaoInternacao = new SolicitacaoInternacaoDto
        {
            Data = new DateOnly(2026, 6, 20),
            IndicacaoClinica = "Cirurgia",
            Observacao = "Obs",
            CIDCodigo = "XYZ99",
            TempoDoenca = "30",
            Diarias = 5,
            Tipo = "Eletiva",
            Regime = "Enfermaria",
            Carater = "Eletivo",
            Local = "Hospital"
        };

        var result = await controller.PostProntuario(dto);

        Assert.IsType<BadRequestObjectResult>(result);
        prontuarioRepo.Verify(r => r.AddAsync(It.IsAny<Prontuario>()), Times.Never);
    }

    // ═══════════════════════════════════════════════════════════
    //  POST /from-pdf
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public void PostFromPdf_Returns501()
    {
        using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            Mock.Of<IProntuarioRepository>(),
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var result = controller.PostFromPDF();

        Assert.Equal(501, (result as StatusCodeResult)?.StatusCode);
    }

    // ═══════════════════════════════════════════════════════════
    //  GET /
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task Get_All_Returns200()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);
        var prontuario = SampleProntuario(paciente.ID, atendimento.Id);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetAllAsync(0, 10)).ReturnsAsync(new[] { prontuario });

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var result = await controller.GetProntuarios();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<ReadProntuarioDto>>(okResult.Value).ToList();
        Assert.Single(dtos);
        Assert.Equal(prontuario.ID, dtos[0].Id);
    }

    // ═══════════════════════════════════════════════════════════
    //  GET /{id}
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task Get_ById_Returns200()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);
        var prontuario = SampleProntuario(paciente.ID, atendimento.Id);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetByIdAsync(prontuario.ID)).ReturnsAsync(prontuario);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var result = await controller.GetByID(prontuario.ID);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ReadProntuarioDto>(okResult.Value);
        Assert.Equal(prontuario.ID, dto.Id);
        Assert.Equal(1, dto.Versao);
    }

    [Fact]
    public async Task Get_ById_Returns404()
    {
        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Prontuario?)null);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var result = await controller.GetByID(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    // ═══════════════════════════════════════════════════════════
    //  GET /paciente/{id}
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task Get_ByPacienteId_Returns200()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);
        var prontuario = SampleProntuario(paciente.ID, atendimento.Id);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetByPacienteIdAsync(paciente.ID)).ReturnsAsync(new List<Prontuario> { prontuario });

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var result = await controller.GetByPacienteId(paciente.ID);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<ReadProntuarioDto>>(okResult.Value).ToList();
        Assert.Single(dtos);
        Assert.Equal(prontuario.ID, dtos[0].Id);
    }

    // ═══════════════════════════════════════════════════════════
    //  GET /paciente/{id}/atual
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task Get_LatestByPaciente_Returns200()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);
        var prontuario = SampleProntuario(paciente.ID, atendimento.Id);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetLatestByPacienteIdAsync(paciente.ID)).ReturnsAsync(prontuario);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var result = await controller.GetLatestByPacienteId(paciente.ID);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<ReadProntuarioDto>(okResult.Value);
    }

    [Fact]
    public async Task Get_LatestByPaciente_Returns404()
    {
        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetLatestByPacienteIdAsync(It.IsAny<Guid>())).ReturnsAsync((Prontuario?)null);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var result = await controller.GetLatestByPacienteId(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    // ═══════════════════════════════════════════════════════════
    //  GET /atendimento/{id}/atual
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task Get_LatestByAtendimento_Returns200()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);
        var prontuario = SampleProntuario(paciente.ID, atendimento.Id);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetLatestByAtendimentoIdAsync(atendimento.Id)).ReturnsAsync(prontuario);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var result = await controller.GetLatestByAtendimentoId(atendimento.Id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<ReadProntuarioDto>(okResult.Value);
    }

    [Fact]
    public async Task Get_LatestByAtendimento_Returns404()
    {
        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetLatestByAtendimentoIdAsync(It.IsAny<Guid>())).ReturnsAsync((Prontuario?)null);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var result = await controller.GetLatestByAtendimentoId(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    // ═══════════════════════════════════════════════════════════
    //  PUT /{id} (correction)
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task Put_ValidCorrection_Returns204()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);
        var prontuario = SampleProntuario(paciente.ID, atendimento.Id);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetByIdAsync(prontuario.ID)).ReturnsAsync(prontuario);
        prontuarioRepo.Setup(r => r.UpdateAsync(It.IsAny<Prontuario>())).Returns(Task.CompletedTask);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var dto = new UpdateProntuarioDto
        {
            InformacoesExtras = "Nota corrigida",
            Profissao = "Médica",
            Religiao = "Espírita",
            AtividadeFisica = "Natação"
        };

        var result = await controller.UpdateProntuario(prontuario.ID, dto);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Put_NotFound_Returns404()
    {
        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Prontuario?)null);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var result = await controller.UpdateProntuario(Guid.NewGuid(), new UpdateProntuarioDto());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Put_NullDto_Returns400()
    {
        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            Mock.Of<IProntuarioRepository>(),
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var result = await controller.UpdateProntuario(Guid.NewGuid(), null!);

        Assert.IsType<BadRequestResult>(result);
    }

    // ═══════════════════════════════════════════════════════════
    //  POST /{id}/versoes (evolution)
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task PostVersoes_ValidEvolution_Returns201()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);
        var v1 = SampleProntuario(paciente.ID, atendimento.Id);

        var pacienteRepo = new Mock<IPacienteRepository>();
        pacienteRepo.Setup(r => r.GetByIdAsync(paciente.ID)).ReturnsAsync(paciente);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetByIdAsync(v1.ID)).ReturnsAsync(v1);
        prontuarioRepo.Setup(r => r.GetMaxVersaoForPacienteAsync(paciente.ID)).ReturnsAsync(1);
        prontuarioRepo.Setup(r => r.AddAsync(It.IsAny<Prontuario>())).Returns(Task.CompletedTask);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            pacienteRepo.Object,
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var dto = new CreateVersaoProntuarioDto
        {
            DataConsulta = new DateOnly(2026, 7, 1),
            Tipo = 1,
            InformacoesExtras = "Evolução",
            DescricaoBasica = new DescricaoBasicaDto
            {
                NomePaciente = "Joana", Cpf = "12345678901", Idade = 37,
                Profissao = "Eng", Religiao = "Cat", QD = "Retorno"
            },
            AGO = new AGODto(),
            Antecedentes = new AntecedentesDto(),
            AntecedentesFamiliares = new AntecedentesFamiliaresDto()
        };

        var result = await controller.CriarNovaVersao(v1.ID, dto);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var readDto = Assert.IsType<ReadProntuarioDto>(created.Value);
        Assert.Equal(paciente.ID, readDto.PacienteId);
        Assert.Equal(atendimento.Id, readDto.AtendimentoId);
        Assert.Equal(2, readDto.Versao);
        Assert.Equal(v1.ID, readDto.ProntuarioAnteriorId);
    }

    [Fact]
    public async Task PostVersoes_SourceNotFound_Returns404()
    {
        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Prontuario?)null);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var result = await controller.CriarNovaVersao(Guid.NewGuid(), new CreateVersaoProntuarioDto());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PostVersoes_InternacaoWithUnknownCid_ReturnsBadRequest()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);
        var v1 = SampleProntuario(paciente.ID, atendimento.Id);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetByIdAsync(v1.ID)).ReturnsAsync(v1);

        await using var context = CreateInMemoryContext(); // No CID seeded
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var dto = new CreateVersaoProntuarioDto
        {
            DataConsulta = new DateOnly(2026, 7, 1),
            Tipo = 1,
            DescricaoBasica = new DescricaoBasicaDto(),
            AGO = new AGODto(),
            Antecedentes = new AntecedentesDto(),
            AntecedentesFamiliares = new AntecedentesFamiliaresDto(),
            SolicitacaoInternacao = new SolicitacaoInternacaoDto
            {
                Data = new DateOnly(2026, 7, 10),
                IndicacaoClinica = "Teste",
                CIDCodigo = "UNKNOWN"
            }
        };

        var result = await controller.CriarNovaVersao(v1.ID, dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PostVersoes_StaleNextVersao_Returns409()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);
        var v1 = SampleProntuario(paciente.ID, atendimento.Id);

        var pacienteRepo = new Mock<IPacienteRepository>();
        pacienteRepo.Setup(r => r.GetByIdAsync(paciente.ID)).ReturnsAsync(paciente);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetByIdAsync(v1.ID)).ReturnsAsync(v1);
        // Return 0 as max — causes nextVersao=1 which is invalid (v1 already has Versao=1)
        prontuarioRepo.Setup(r => r.GetMaxVersaoForPacienteAsync(paciente.ID)).ReturnsAsync(0);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            pacienteRepo.Object,
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var dto = new CreateVersaoProntuarioDto
        {
            DataConsulta = new DateOnly(2026, 7, 1),
            Tipo = 1,
            DescricaoBasica = new DescricaoBasicaDto(),
            AGO = new AGODto(),
            Antecedentes = new AntecedentesDto(),
            AntecedentesFamiliares = new AntecedentesFamiliaresDto()
        };

        var result = await controller.CriarNovaVersao(v1.ID, dto);

        Assert.IsType<ConflictResult>(result);
    }

    [Fact]
    public async Task PostVersoes_DbUpdateUniqueViolation_Returns409()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);
        var v1 = SampleProntuario(paciente.ID, atendimento.Id);

        var pacienteRepo = new Mock<IPacienteRepository>();
        pacienteRepo.Setup(r => r.GetByIdAsync(paciente.ID)).ReturnsAsync(paciente);

        var innerException = new Exception("Violation of UNIQUE constraint IX_Prontuario_PacienteId_Versao");
        var dbException = new DbUpdateException("Erro de banco", innerException);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetByIdAsync(v1.ID)).ReturnsAsync(v1);
        prontuarioRepo.Setup(r => r.GetMaxVersaoForPacienteAsync(paciente.ID)).ReturnsAsync(5);
        prontuarioRepo.Setup(r => r.AddAsync(It.IsAny<Prontuario>())).Throws(dbException);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            pacienteRepo.Object,
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var dto = new CreateVersaoProntuarioDto
        {
            DataConsulta = new DateOnly(2026, 7, 1),
            Tipo = 1,
            DescricaoBasica = new DescricaoBasicaDto(),
            AGO = new AGODto(),
            Antecedentes = new AntecedentesDto(),
            AntecedentesFamiliares = new AntecedentesFamiliaresDto()
        };

        var result = await controller.CriarNovaVersao(v1.ID, dto);

        Assert.IsType<ConflictResult>(result);
    }

    // ═══════════════════════════════════════════════════════════
    //  DELETE /{id}
    // ═══════════════════════════════════════════════════════════

    [Fact]
    public async Task Delete_Existing_Returns204()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);
        var prontuario = SampleProntuario(paciente.ID, atendimento.Id);

        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetByIdAsync(prontuario.ID)).ReturnsAsync(prontuario);
        prontuarioRepo.Setup(r => r.DeleteAsync(It.IsAny<Prontuario>())).Returns(Task.CompletedTask);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var result = await controller.DeleteProntuario(prontuario.ID);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_NotFound_Returns404()
    {
        var prontuarioRepo = new Mock<IProntuarioRepository>();
        prontuarioRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Prontuario?)null);

        await using var context = CreateInMemoryContext();
        var controller = new ProntuarioController(
            prontuarioRepo.Object,
            Mock.Of<IPacienteRepository>(),
            Mock.Of<IAtendimentoRepository>(),
            context,
            CreateMapper());

        var result = await controller.DeleteProntuario(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }
}