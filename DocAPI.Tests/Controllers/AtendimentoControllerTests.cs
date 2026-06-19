using AutoMapper;
using DocAPI.Controllers;
using DocAPI.Core.Entities;
using DocAPI.Core.Interfaces.Repositories;
using DocAPI.Data.Dtos.Atendimento;
using DocAPI.Interfaces.Repositories;
using DocAPI.Profiles;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DocAPI.Tests.Controllers;

public class AtendimentoControllerTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AtendimentoProfile>());
        return config.CreateMapper();
    }

    [Fact]
    public async Task Post_InvalidPacienteId_ReturnsNotFound()
    {
        var pacienteId = Guid.NewGuid();
        var pacienteRepo = new Mock<IPacienteRepository>();
        pacienteRepo.Setup(r => r.GetByIdAsync(pacienteId)).ReturnsAsync((Paciente?)null);

        var atendimentoRepo = new Mock<IAtendimentoRepository>();
        var controller = new AtendimentoController(
            atendimentoRepo.Object,
            pacienteRepo.Object,
            CreateMapper());

        var result = await controller.Post(new CreateAtendimentoDto
        {
            PacienteId = pacienteId,
            MensagemParaMedico = "Teste"
        });

        Assert.IsType<NotFoundResult>(result);
        atendimentoRepo.Verify(r => r.CreateAsync(It.IsAny<Atendimento>()), Times.Never);
    }

    [Fact]
    public async Task Post_ValidPacienteId_ReturnsCreated()
    {
        var paciente = new Paciente(
            "Paciente Controller",
            new DateOnly(1990, 1, 1),
            "12345678901",
            "controller@example.com",
            "31990001111");

        var pacienteRepo = new Mock<IPacienteRepository>();
        pacienteRepo.Setup(r => r.GetByIdAsync(paciente.ID)).ReturnsAsync(paciente);

        var atendimentoRepo = new Mock<IAtendimentoRepository>();
        atendimentoRepo
            .Setup(r => r.CreateAsync(It.IsAny<Atendimento>()))
            .Returns(Task.CompletedTask);

        var controller = new AtendimentoController(
            atendimentoRepo.Object,
            pacienteRepo.Object,
            CreateMapper());

        var result = await controller.Post(new CreateAtendimentoDto
        {
            PacienteId = paciente.ID,
            MensagemParaMedico = "Mensagem"
        });

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var dto = Assert.IsType<ReadAtendimentoDto>(created.Value);
        Assert.Equal(paciente.ID, dto.PacienteId);
        Assert.Equal(Atendimento.EtapaAtendimento.Consulta, dto.EtapaAtual);
        Assert.Equal("Mensagem", dto.MensagemParaMedico);
    }

    [Fact]
    public void ReportRoutes_Return501()
    {
        var controller = new AtendimentoController(
            Mock.Of<IAtendimentoRepository>(),
            Mock.Of<IPacienteRepository>(),
            CreateMapper());

        var report = controller.GetPatientReportPdf(Guid.NewGuid());
        var followUp = controller.GetPatientFollowUp(Guid.NewGuid());

        Assert.Equal(501, (report as StatusCodeResult)?.StatusCode);
        Assert.Equal(501, (followUp as StatusCodeResult)?.StatusCode);
    }
}
