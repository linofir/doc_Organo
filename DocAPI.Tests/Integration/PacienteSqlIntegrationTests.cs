using DocAPI.Core.Entities;
using DocAPI.Infrastructure.Repositories;
using DocAPI.Infrastructure.SqlDb;
using DocAPI.Infrastructure.SqlDb.Context;
using DocAPI.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DocAPI.Tests.Integration;

public class PacienteSqlIntegrationTests
{

    private static DocDbContext CreateSqlContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<DocDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        return new DocDbContext(options);
    }

    private static string GenerateSyntheticCpf()
    {
        return $"{Random.Shared.Next(100000000, 999999999)}{Random.Shared.Next(10, 99)}";
    }

    private static Paciente BuildSyntheticPaciente(string cpf, string suffix)
    {
        var paciente = new Paciente(
            $"Paciente Teste {suffix}",
            new DateOnly(1990, 1, 15),
            cpf,
            $"teste.{suffix}@example.com",
            "31990001111");
        paciente.ComplementarCadastro(
            $"MG{suffix}",
            "Plano Teste",
            $"CART{suffix}",
            new Endereco
            {
                Logradouro = "Rua Integracao",
                Numero = "42",
                Bairro = "Teste",
                Cidade = "Belo Horizonte",
                UF = "MG",
                CEP = "30130000"
            });
        return paciente;
    }

    [SkippableFact]
    public async Task PacienteSql_CreateReadUpdateSoftDelete_RoundTrip()
    {
        var skipReason = await SqlIntegrationTestGate.GetSkipReasonAsync();
        Skip.If(skipReason is not null, skipReason!);

        var connectionString = SqlConnectionResolver.ResolveConnectionString()!;
        await using var context = CreateSqlContext(connectionString);
        var repo = new PacienteRepository(context);
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var cpf = GenerateSyntheticCpf();
        var paciente = BuildSyntheticPaciente(cpf, suffix);

        await repo.CreateAsync(paciente);

        var created = await repo.GetByIdAsync(paciente.ID);
        Assert.NotNull(created);
        Assert.Equal("Plano Teste", created!.Plano);
        Assert.Equal("Rua Integracao", created.Endereco!.Logradouro);

        var updated = BuildSyntheticPaciente(cpf, $"{suffix}-upd");
        await repo.UpdateAsync(updated, paciente.ID);

        var afterUpdate = await repo.GetByIdAsync(paciente.ID);
        Assert.NotNull(afterUpdate);
        Assert.Equal($"Paciente Teste {suffix}-upd", afterUpdate!.Nome);
        Assert.Equal("42", afterUpdate.Endereco!.Numero);

        await repo.DeleteAsync(paciente.ID);

        Assert.Null(await repo.GetByIdAsync(paciente.ID));
        Assert.Empty(await repo.GetPacienteByCpfAsync(cpf));
        Assert.Empty(await repo.GetPacienteByNomeAsync(suffix));
    }
}
