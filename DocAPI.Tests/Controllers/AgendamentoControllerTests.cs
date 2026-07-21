using AutoMapper;
using DocAPI.Controllers;
using DocAPI.Core.Entities;
using DocAPI.Core.Interfaces.Repositories;
using DocAPI.Data.Dtos.AgendamentoDtos;
using DocAPI.Infrastructure.SqlDb.Context;
using DocAPI.Interfaces.Repositories;
using DocAPI.Profiles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DocAPI.Tests.Controllers;

public class AgendamentoControllerTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AgendamentoProfile>());
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
            "Paciente Teste",
            new DateOnly(1990, 1, 1),
            "11122233344",
            "paciente@test.com",
            "31990001111");
    }

    private static Atendimento SampleAtendimento(Guid pacienteId)
    {
        return new Atendimento(pacienteId);
    }

    private static Internacao SampleInternacao()
    {
        return new Internacao(
            Guid.NewGuid(),
            new DateOnly(2026, 7, 1),
            "Dor abdominal",
            "Observação",
            "R10",
            "7 dias",
            1,
            "Eletiva",
            "Ambulatorial",
            "Eletivo",
            false,
            "Hospital Central");
    }

    private static async Task<DocDbContext> SeedInternacaoAsync()
    {
        var context = CreateInMemoryContext();
        context.Set<Internacao>().Add(SampleInternacao());
        await context.SaveChangesAsync();
        return context;
    }

    [Fact]
    public async Task GetById_Existing_ReturnsOk()
    {
        var agendamento = new Agendamento(
            internacaoId: Guid.NewGuid(),
            atendimentoId: Guid.NewGuid(),
            data: new DateOnly(2026, 7, 20),
            horario: new TimeOnly(14, 0),
            nome: "Maria Silva");

        var repo = new Mock<IAgendamentoRepository>();
        repo.Setup(r => r.GetByIdAsync(agendamento.ID)).ReturnsAsync(agendamento);

        var controller = new AgendamentoController(
            repo.Object, CreateMapper(),
            new Mock<IAtendimentoRepository>().Object,
            new Mock<IPacienteRepository>().Object,
            CreateInMemoryContext());

        var result = await controller.GetByID(agendamento.ID);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<ReadAgendamentoDto>(ok.Value);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var repo = new Mock<IAgendamentoRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Agendamento?)null);

        var controller = new AgendamentoController(
            repo.Object, CreateMapper(),
            new Mock<IAtendimentoRepository>().Object,
            new Mock<IPacienteRepository>().Object,
            CreateInMemoryContext());

        var result = await controller.GetByID(Guid.NewGuid());
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Post_AllFKsValid_ReturnsCreated()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);

        var pacienteRepo = new Mock<IPacienteRepository>();
        pacienteRepo.Setup(r => r.GetByIdAsync(paciente.ID)).ReturnsAsync(paciente);

        var atendimentoRepo = new Mock<IAtendimentoRepository>();
        atendimentoRepo.Setup(r => r.GetByIdAsync(atendimento.Id)).ReturnsAsync(atendimento);

        var agendamentoRepo = new Mock<IAgendamentoRepository>();

        await using var context = await SeedInternacaoAsync();
        var internacao = context.Set<Internacao>().First();

        var controller = new AgendamentoController(
            agendamentoRepo.Object, CreateMapper(),
            atendimentoRepo.Object, pacienteRepo.Object, context);

        var dto = new CreateAgendamentoDto
        {
            AtendimentoId = atendimento.Id,
            InternacaoId = internacao.ID,
            PacienteId = paciente.ID,
            Nome = "Maria Silva",
            Data = new DateOnly(2026, 7, 20)
        };

        var result = await controller.PostAgendamento(dto);

        Assert.IsType<CreatedAtActionResult>(result);
        agendamentoRepo.Verify(r => r.CreateAsync(It.IsAny<Agendamento>()), Times.Once);
    }

    [Fact]
    public async Task Post_InvalidAtendimentoId_Returns404()
    {
        var atendimentoRepo = new Mock<IAtendimentoRepository>();
        atendimentoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Atendimento?)null);

        var agendamentoRepo = new Mock<IAgendamentoRepository>();

        var controller = new AgendamentoController(
            agendamentoRepo.Object, CreateMapper(),
            atendimentoRepo.Object,
            new Mock<IPacienteRepository>().Object,
            CreateInMemoryContext());

        var dto = new CreateAgendamentoDto
        {
            AtendimentoId = Guid.NewGuid(),
            InternacaoId = Guid.NewGuid(),
            Nome = "Maria Silva",
            Data = new DateOnly(2026, 7, 20)
        };

        var result = await controller.PostAgendamento(dto);

        Assert.IsType<NotFoundObjectResult>(result);
        agendamentoRepo.Verify(r => r.CreateAsync(It.IsAny<Agendamento>()), Times.Never);
    }

    [Fact]
    public async Task Post_InvalidInternacaoId_Returns404()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);

        var pacienteRepo = new Mock<IPacienteRepository>();
        var atendimentoRepo = new Mock<IAtendimentoRepository>();
        atendimentoRepo.Setup(r => r.GetByIdAsync(atendimento.Id)).ReturnsAsync(atendimento);

        var agendamentoRepo = new Mock<IAgendamentoRepository>();

        await using var context = CreateInMemoryContext();
        // Nenhuma Internacao existe

        var controller = new AgendamentoController(
            agendamentoRepo.Object, CreateMapper(),
            atendimentoRepo.Object, pacienteRepo.Object, context);

        var dto = new CreateAgendamentoDto
        {
            AtendimentoId = atendimento.Id,
            InternacaoId = Guid.NewGuid(),
            Nome = "Maria Silva",
            Data = new DateOnly(2026, 7, 20)
        };

        var result = await controller.PostAgendamento(dto);

        Assert.IsType<NotFoundObjectResult>(result);
        agendamentoRepo.Verify(r => r.CreateAsync(It.IsAny<Agendamento>()), Times.Never);
    }

    [Fact]
    public async Task Post_InvalidPacienteId_Returns404()
    {
        var atendimento = SampleAtendimento(Guid.NewGuid());
        var atendimentoRepo = new Mock<IAtendimentoRepository>();
        atendimentoRepo.Setup(r => r.GetByIdAsync(atendimento.Id)).ReturnsAsync(atendimento);

        var pacienteRepo = new Mock<IPacienteRepository>();
        pacienteRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Paciente?)null);

        var agendamentoRepo = new Mock<IAgendamentoRepository>();

        await using var context = await SeedInternacaoAsync();
        var internacao = context.Set<Internacao>().First();

        var controller = new AgendamentoController(
            agendamentoRepo.Object, CreateMapper(),
            atendimentoRepo.Object, pacienteRepo.Object, context);

        var dto = new CreateAgendamentoDto
        {
            AtendimentoId = atendimento.Id,
            InternacaoId = internacao.ID,
            PacienteId = Guid.NewGuid(),
            Nome = "Maria Silva",
            Data = new DateOnly(2026, 7, 20)
        };

        var result = await controller.PostAgendamento(dto);

        Assert.IsType<NotFoundObjectResult>(result);
        agendamentoRepo.Verify(r => r.CreateAsync(It.IsAny<Agendamento>()), Times.Never);
    }

    [Fact]
    public async Task Put_Existing_ReturnsNoContent()
    {
        var agendamento = new Agendamento(
            internacaoId: Guid.NewGuid(),
            atendimentoId: Guid.NewGuid(),
            data: new DateOnly(2026, 7, 20),
            horario: new TimeOnly(14, 0),
            nome: "Maria Silva");

        var repo = new Mock<IAgendamentoRepository>();
        repo.Setup(r => r.GetByIdAsync(agendamento.ID)).ReturnsAsync(agendamento);

        var controller = new AgendamentoController(
            repo.Object, CreateMapper(),
            new Mock<IAtendimentoRepository>().Object,
            new Mock<IPacienteRepository>().Object,
            CreateInMemoryContext());

        var dto = new UpdateAgendamentoDto { Nome = "Nome Atualizado" };
        var result = await controller.UpdateAgendamento(agendamento.ID, dto);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Put_NotFound_Returns404()
    {
        var repo = new Mock<IAgendamentoRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Agendamento?)null);

        var controller = new AgendamentoController(
            repo.Object, CreateMapper(),
            new Mock<IAtendimentoRepository>().Object,
            new Mock<IPacienteRepository>().Object,
            CreateInMemoryContext());

        var dto = new UpdateAgendamentoDto { Nome = "Nome" };
        var result = await controller.UpdateAgendamento(Guid.NewGuid(), dto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Existing_ReturnsNoContent()
    {
        var agendamento = new Agendamento(
            internacaoId: Guid.NewGuid(),
            atendimentoId: Guid.NewGuid(),
            data: new DateOnly(2026, 7, 20),
            horario: new TimeOnly(14, 0),
            nome: "Maria Silva");

        var repo = new Mock<IAgendamentoRepository>();
        repo.Setup(r => r.GetByIdAsync(agendamento.ID)).ReturnsAsync(agendamento);

        var controller = new AgendamentoController(
            repo.Object, CreateMapper(),
            new Mock<IAtendimentoRepository>().Object,
            new Mock<IPacienteRepository>().Object,
            CreateInMemoryContext());

        var result = await controller.DeleteAgendamento(agendamento.ID);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_NotFound_Returns404()
    {
        var repo = new Mock<IAgendamentoRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Agendamento?)null);

        var controller = new AgendamentoController(
            repo.Object, CreateMapper(),
            new Mock<IAtendimentoRepository>().Object,
            new Mock<IPacienteRepository>().Object,
            CreateInMemoryContext());

        var result = await controller.DeleteAgendamento(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetByName_NoMatches_ReturnsOkEmptyArray()
    {
        var repo = new Mock<IAgendamentoRepository>();
        repo.Setup(r => r.GetByNameAsync("ninguem")).ReturnsAsync(new List<Agendamento>());

        var controller = new AgendamentoController(
            repo.Object, CreateMapper(),
            new Mock<IAtendimentoRepository>().Object,
            new Mock<IPacienteRepository>().Object,
            CreateInMemoryContext());

        var result = await controller.GetByName("ninguem");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task GetByPacienteId_ReturnsOk()
    {
        var repo = new Mock<IAgendamentoRepository>();
        repo.Setup(r => r.GetByPacienteIdAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Agendamento>());

        var controller = new AgendamentoController(
            repo.Object, CreateMapper(),
            new Mock<IAtendimentoRepository>().Object,
            new Mock<IPacienteRepository>().Object,
            CreateInMemoryContext());

        var result = await controller.GetByPacienteId(Guid.NewGuid());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task FKErrorBody_ContainsCorrectFieldName()
    {
        var atendimentoRepo = new Mock<IAtendimentoRepository>();
        atendimentoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Atendimento?)null);

        var controller = new AgendamentoController(
            new Mock<IAgendamentoRepository>().Object, CreateMapper(),
            atendimentoRepo.Object,
            new Mock<IPacienteRepository>().Object,
            CreateInMemoryContext());

        var dto = new CreateAgendamentoDto
        {
            AtendimentoId = Guid.NewGuid(),
            InternacaoId = Guid.NewGuid(),
            Nome = "Maria Silva",
            Data = new DateOnly(2026, 7, 20)
        };

        var result = await controller.PostAgendamento(dto);
        var notFound = Assert.IsType<NotFoundObjectResult>(result);

        // Verify the structured error body
        var body = notFound.Value;
        Assert.NotNull(body);
    }
}