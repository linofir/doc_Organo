using Xunit;
using DocAPI.Core.Entities;
using DocAPI.Infrastructure.Repositories;
using DocAPI.Infrastructure.SqlDb.Context;
using Microsoft.EntityFrameworkCore;

namespace DocAPI.Tests.Infrastructure;

public class PacienteRepositoryTests
{
    private static DocDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<DocDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new DocDbContext(options);
    }

    private static Paciente SamplePaciente(string cpf = "12345678901") =>
        new("Maria Silva", new DateOnly(1990, 5, 15), cpf, "maria@example.com", "11999999999");

    [Fact]
    public async Task CreateAsync_PersistsPaciente()
    {
        await using var context = CreateContext(nameof(CreateAsync_PersistsPaciente));
        var repo = new PacienteRepository(context);
        var paciente = SamplePaciente();

        await repo.CreateAsync(paciente);

        var saved = await context.Pacientes.FirstOrDefaultAsync(p => p.ID == paciente.ID);
        Assert.NotNull(saved);
        Assert.Equal("Maria Silva", saved.Nome);
        Assert.NotEqual(default, saved.CriadoEm);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsPaciente()
    {
        await using var context = CreateContext(nameof(GetByIdAsync_ReturnsPaciente));
        var repo = new PacienteRepository(context);
        var paciente = SamplePaciente();
        await repo.CreateAsync(paciente);

        var found = await repo.GetByIdAsync(paciente.ID);

        Assert.NotNull(found);
        Assert.Equal(paciente.ID, found.ID);
    }

    [Fact]
    public async Task GetPacienteByCpfAsync_FindsByCpf()
    {
        await using var context = CreateContext(nameof(GetPacienteByCpfAsync_FindsByCpf));
        var repo = new PacienteRepository(context);
        await repo.CreateAsync(SamplePaciente("98765432100"));

        var results = await repo.GetPacienteByCpfAsync("98765432100");

        Assert.Single(results);
        Assert.Equal("98765432100", results[0].CPF);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesPaciente()
    {
        await using var context = CreateContext(nameof(DeleteAsync_SoftDeletesPaciente));
        var repo = new PacienteRepository(context);
        var paciente = SamplePaciente();
        await repo.CreateAsync(paciente);

        await repo.DeleteAsync(paciente.ID);

        var found = await repo.GetByIdAsync(paciente.ID);
        Assert.Null(found);

        var includingDeleted = await context.Pacientes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.ID == paciente.ID);
        Assert.NotNull(includingDeleted);
        Assert.True(includingDeleted.Deletado);
    }

    [Fact]
    public async Task UpdateAsync_ChangesFields()
    {
        await using var context = CreateContext(nameof(UpdateAsync_ChangesFields));
        var repo = new PacienteRepository(context);
        var paciente = SamplePaciente();
        await repo.CreateAsync(paciente);

        var updated = new Paciente(
            "Maria Santos",
            new DateOnly(1990, 5, 15),
            paciente.CPF,
            "maria.santos@example.com",
            "11888888888");

        await repo.UpdateAsync(updated, paciente.ID);

        var found = await repo.GetByIdAsync(paciente.ID);
        Assert.NotNull(found);
        Assert.Equal("Maria Santos", found.Nome);
        Assert.Equal("maria.santos@example.com", found.Email);
        Assert.NotNull(found.AtualizadoEm);
    }
}
