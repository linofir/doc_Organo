using DocAPI.Core.Entities;
using DocAPI.Infrastructure.Repositories;
using DocAPI.Infrastructure.SqlDb.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DocAPI.Tests.Infrastructure;

public class AtendimentoRepositoryTests
{
    private static DocDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<DocDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new DocDbContext(options);
    }

    private static async Task<Paciente> SeedPacienteAsync(DocDbContext context, string suffix = "a")
    {
        var paciente = new Paciente(
            $"Paciente Teste {suffix}",
            new DateOnly(1990, 5, 15),
            $"1234567890{suffix[0]}",
            $"teste.{suffix}@example.com",
            "11999999999");
        paciente.AplicarCriacao();
        context.Pacientes.Add(paciente);
        await context.SaveChangesAsync();
        return paciente;
    }

    [Fact]
    public async Task CreateAsync_PersistsAtendimentoWithConsultaStage()
    {
        await using var context = CreateContext(nameof(CreateAsync_PersistsAtendimentoWithConsultaStage));
        var paciente = await SeedPacienteAsync(context);
        var repo = new AtendimentoRepository(context);
        var atendimento = new Atendimento(paciente.ID, "Mensagem inicial");

        await repo.CreateAsync(atendimento);

        var saved = await context.Atendimentos.FirstOrDefaultAsync(a => a.Id == atendimento.Id);
        Assert.NotNull(saved);
        Assert.Equal(paciente.ID, saved.PacienteId);
        Assert.Equal(Atendimento.EtapaAtendimento.Consulta, saved.EtapaAtual);
        Assert.Equal("Mensagem inicial", saved.MensagemParaMedico);
        Assert.NotEqual(default, saved.CriadoEm);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsAtendimento()
    {
        await using var context = CreateContext(nameof(GetByIdAsync_ReturnsAtendimento));
        var paciente = await SeedPacienteAsync(context);
        var repo = new AtendimentoRepository(context);
        var atendimento = new Atendimento(paciente.ID);
        await repo.CreateAsync(atendimento);

        var found = await repo.GetByIdAsync(atendimento.Id);

        Assert.NotNull(found);
        Assert.Equal(atendimento.Id, found!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullForMissing()
    {
        await using var context = CreateContext(nameof(GetByIdAsync_ReturnsNullForMissing));
        var repo = new AtendimentoRepository(context);

        var found = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(found);
    }

    [Fact]
    public async Task GetByPacienteIdAsync_ReturnsAllForPatient()
    {
        await using var context = CreateContext(nameof(GetByPacienteIdAsync_ReturnsAllForPatient));
        var paciente = await SeedPacienteAsync(context);
        var otherPaciente = await SeedPacienteAsync(context, "b");
        var repo = new AtendimentoRepository(context);

        var first = new Atendimento(paciente.ID, "Primeiro");
        var second = new Atendimento(paciente.ID, "Segundo");
        var other = new Atendimento(otherPaciente.ID);
        await repo.CreateAsync(first);
        await repo.CreateAsync(second);
        await repo.CreateAsync(other);

        var results = (await repo.GetByPacienteIdAsync(paciente.ID)).ToList();

        Assert.Equal(2, results.Count);
        Assert.All(results, a => Assert.Equal(paciente.ID, a.PacienteId));
    }

    [Fact]
    public async Task GetByPacienteIdAsync_ReturnsEmptyWhenNoneExist()
    {
        await using var context = CreateContext(nameof(GetByPacienteIdAsync_ReturnsEmptyWhenNoneExist));
        var repo = new AtendimentoRepository(context);

        var results = await repo.GetByPacienteIdAsync(Guid.NewGuid());

        Assert.Empty(results);
    }

    [Fact]
    public async Task UpdateAsync_ChangesMessageOnly()
    {
        await using var context = CreateContext(nameof(UpdateAsync_ChangesMessageOnly));
        var paciente = await SeedPacienteAsync(context);
        var repo = new AtendimentoRepository(context);
        var atendimento = new Atendimento(paciente.ID, "Antes");
        await repo.CreateAsync(atendimento);

        var atualizado = new Atendimento(paciente.ID, "Depois");
        await repo.UpdateAsync(atualizado, atendimento.Id);

        var found = await repo.GetByIdAsync(atendimento.Id);
        Assert.NotNull(found);
        Assert.Equal("Depois", found!.MensagemParaMedico);
        Assert.Equal(Atendimento.EtapaAtendimento.Consulta, found.EtapaAtual);
        Assert.NotNull(found.AtualizadoEm);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesAtendimento()
    {
        await using var context = CreateContext(nameof(DeleteAsync_SoftDeletesAtendimento));
        var paciente = await SeedPacienteAsync(context);
        var repo = new AtendimentoRepository(context);
        var atendimento = new Atendimento(paciente.ID);
        await repo.CreateAsync(atendimento);

        await repo.DeleteAsync(atendimento.Id);

        Assert.Null(await repo.GetByIdAsync(atendimento.Id));
        Assert.Empty(await repo.GetByPacienteIdAsync(paciente.ID));

        var includingDeleted = await context.Atendimentos
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == atendimento.Id);
        Assert.NotNull(includingDeleted);
        Assert.True(includingDeleted!.Deletado);
    }

    [Fact]
    public async Task GetAllAsync_PaginatesResults()
    {
        await using var context = CreateContext(nameof(GetAllAsync_PaginatesResults));
        var paciente = await SeedPacienteAsync(context);
        var repo = new AtendimentoRepository(context);
        await repo.CreateAsync(new Atendimento(paciente.ID, "Um"));
        await repo.CreateAsync(new Atendimento(paciente.ID, "Dois"));
        await repo.CreateAsync(new Atendimento(paciente.ID, "Tres"));

        var page = (await repo.GetAllAsync(skip: 1, take: 1)).ToList();

        Assert.Single(page);
    }
}
