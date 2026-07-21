using DocAPI.Core.Entities;
using DocAPI.Infrastructure.Repositories;
using DocAPI.Infrastructure.SqlDb.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DocAPI.Tests.Infrastructure;

public class AgendamentoRepositoryTests
{
    private static DocDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<DocDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new DocDbContext(options);
    }

    private static Agendamento SampleAgendamento(string nome = "Maria Silva")
    {
        var internacao = new Internacao(
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
        var atendimento = new Atendimento(Guid.NewGuid());

        return new Agendamento(
            internacaoId: internacao.ID,
            atendimentoId: atendimento.Id,
            data: new DateOnly(2026, 7, 20),
            horario: new TimeOnly(14, 0),
            pacienteID: null,
            nome: nome,
            aviso: "Jejum 8h",
            local: "Hospital Central",
            sala: "3A");
    }

    [Fact]
    public async Task CreateAsync_ThenGetById_ReturnsAgendamento()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new AgendamentoRepository(context);
        var agendamento = SampleAgendamento();

        await repo.CreateAsync(agendamento);
        var result = await repo.GetByIdAsync(agendamento.ID);

        Assert.NotNull(result);
        Assert.Equal(agendamento.ID, result!.ID);
        Assert.Equal("Maria Silva", result.Nome);
    }

    [Fact]
    public async Task GetAll_ReturnsPaginatedResults()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new AgendamentoRepository(context);

        await repo.CreateAsync(SampleAgendamento("A"));
        await repo.CreateAsync(SampleAgendamento("B"));
        await repo.CreateAsync(SampleAgendamento("C"));

        var result = await repo.GetAllAsync(0, 2);
        var list = result.ToList();

        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetAll_TakeExceeds100_ClampsAt100()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new AgendamentoRepository(context);

        for (int i = 0; i < 150; i++)
            await repo.CreateAsync(SampleAgendamento($"Nome{i}"));

        var result = await repo.GetAllAsync(0, 200);
        Assert.True(result.Count() <= 100);
    }

    [Fact]
    public async Task GetByName_ReturnsMatchingSubstring()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new AgendamentoRepository(context);

        await repo.CreateAsync(SampleAgendamento("João Cardoso"));
        await repo.CreateAsync(SampleAgendamento("Ana Maria"));

        var result = await repo.GetByNameAsync("Maria");
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByPacienteId_ReturnsMatchingAgendamentos()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new AgendamentoRepository(context);

        var pacienteID = Guid.NewGuid();
        var a1 = new Agendamento(
            internacaoId: Guid.NewGuid(),
            atendimentoId: Guid.NewGuid(),
            data: new DateOnly(2026, 7, 20),
            horario: new TimeOnly(14, 0),
            pacienteID: pacienteID,
            nome: "Com Paciente");

        var a2 = new Agendamento(
            internacaoId: Guid.NewGuid(),
            atendimentoId: Guid.NewGuid(),
            data: new DateOnly(2026, 7, 21),
            horario: new TimeOnly(15, 0),
            pacienteID: null,
            nome: "Sem Paciente");

        await repo.CreateAsync(a1);
        await repo.CreateAsync(a2);

        var result = await repo.GetByPacienteIdAsync(pacienteID);
        Assert.Single(result);
        Assert.Equal("Com Paciente", result[0].Nome);
    }

    [Fact]
    public async Task UpdateAsync_PersistsChanges()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new AgendamentoRepository(context);
        var agendamento = SampleAgendamento();

        await repo.CreateAsync(agendamento);

        agendamento.Update(nome: "Nome Atualizado");
        await repo.UpdateAsync(agendamento, agendamento.ID);

        var result = await repo.GetByIdAsync(agendamento.ID);
        Assert.Equal("Nome Atualizado", result!.Nome);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesAndExcludesFromQueries()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new AgendamentoRepository(context);
        var agendamento = SampleAgendamento();

        await repo.CreateAsync(agendamento);
        await repo.DeleteAsync(agendamento.ID);

        var result = await repo.GetByIdAsync(agendamento.ID);
        Assert.Null(result);
    }
}