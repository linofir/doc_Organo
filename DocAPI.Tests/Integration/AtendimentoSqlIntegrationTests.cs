using DocAPI.Core.Entities;
using DocAPI.Infrastructure.Repositories;
using DocAPI.Infrastructure.SqlDb;
using DocAPI.Infrastructure.SqlDb.Context;
using DocAPI.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DocAPI.Tests.Integration;

public class AtendimentoSqlIntegrationTests
{

    private static DocDbContext CreateSqlContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<DocDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        return new DocDbContext(options);
    }

    private static string GenerateSyntheticCpf() =>
        $"{Random.Shared.Next(100000000, 999999999)}{Random.Shared.Next(10, 99)}";

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
    public async Task AtendimentoSql_CreateReadUpdateSoftDelete_RoundTrip()
    {
        var skipReason = await SqlIntegrationTestGate.GetSkipReasonAsync();
        Skip.If(skipReason is not null, skipReason!);

        var connectionString = SqlConnectionResolver.ResolveConnectionString()!;
        await using var context = CreateSqlContext(connectionString);
        var pacienteRepo = new PacienteRepository(context);
        var atendimentoRepo = new AtendimentoRepository(context);

        var suffix = Guid.NewGuid().ToString("N")[..8];
        var paciente = BuildSyntheticPaciente(GenerateSyntheticCpf(), suffix);
        await pacienteRepo.CreateAsync(paciente);

        var atendimento = new Atendimento(paciente.ID, "Mensagem integracao");
        await atendimentoRepo.CreateAsync(atendimento);

        var created = await atendimentoRepo.GetByIdAsync(atendimento.Id);
        Assert.NotNull(created);
        Assert.Equal(Atendimento.EtapaAtendimento.Consulta, created!.EtapaAtual);
        Assert.Equal("Mensagem integracao", created.MensagemParaMedico);

        var byPaciente = (await atendimentoRepo.GetByPacienteIdAsync(paciente.ID)).ToList();
        Assert.Contains(byPaciente, a => a.Id == atendimento.Id);

        var updated = new Atendimento(paciente.ID, "Mensagem atualizada");
        await atendimentoRepo.UpdateAsync(updated, atendimento.Id);

        var afterUpdate = await atendimentoRepo.GetByIdAsync(atendimento.Id);
        Assert.NotNull(afterUpdate);
        Assert.Equal("Mensagem atualizada", afterUpdate!.MensagemParaMedico);
        Assert.Equal(Atendimento.EtapaAtendimento.Consulta, afterUpdate.EtapaAtual);

        await atendimentoRepo.DeleteAsync(atendimento.Id);

        Assert.Null(await atendimentoRepo.GetByIdAsync(atendimento.Id));
        Assert.DoesNotContain(
            (await atendimentoRepo.GetByPacienteIdAsync(paciente.ID)).Select(a => a.Id),
            id => id == atendimento.Id);
    }
}
