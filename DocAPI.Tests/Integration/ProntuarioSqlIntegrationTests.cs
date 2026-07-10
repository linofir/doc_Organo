using DocAPI.Core.Entities;
using DocAPI.Infrastructure.Repositories;
using DocAPI.Infrastructure.SqlDb;
using DocAPI.Infrastructure.SqlDb.Context;
using DocAPI.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DocAPI.Tests.Integration;

public class ProntuarioSqlIntegrationTests
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
            $"Paciente Pront Teste {suffix}",
            new DateOnly(1990, 1, 15),
            cpf,
            $"pront.{suffix}@example.com",
            "31990002222");
        paciente.ComplementarCadastro(
            $"MG{suffix}",
            "Plano Teste",
            $"CART{suffix}",
            new Endereco
            {
                Logradouro = "Rua Prontuario",
                Numero = "100",
                Bairro = "Teste",
                Cidade = "Belo Horizonte",
                UF = "MG",
                CEP = "30130000"
            });
        return paciente;
    }

    private static CID BuildSyntheticCid(string codigo = "Z99.9")
    {
        return new CID { Codigo = codigo, Descricao = "CID sintético para teste de integração" };
    }

    [SkippableFact]
    public async Task ProntuarioSql_FullVersioningChain_WithInternacao_RoundTrip()
    {
        var skipReason = await SqlIntegrationTestGate.GetSkipReasonAsync();
        Skip.If(skipReason is not null, skipReason!);

        var connectionString = SqlConnectionResolver.ResolveConnectionString()!;
        await using var context = CreateSqlContext(connectionString);
        var pacienteRepo = new PacienteRepository(context);
        var atendimentoRepo = new AtendimentoRepository(context);
        var prontuarioRepo = new ProntuarioRepository(context);

        // Seed synthetic CID
        var cid = BuildSyntheticCid();
        if (!await context.CIDs.AnyAsync(c => c.Codigo == cid.Codigo))
        {
            context.CIDs.Add(cid);
            await context.SaveChangesAsync();
        }

        // Create paciente
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var paciente = BuildSyntheticPaciente(GenerateSyntheticCpf(), suffix);
        await pacienteRepo.CreateAsync(paciente);

        // Create atendimento
        var atendimento = new Atendimento(paciente.ID, "Integração prontuário");
        await atendimentoRepo.CreateAsync(atendimento);

        // Create prontuario v1 with nested Exames, CD, and Internacao
        var v1 = new Prontuario(paciente.ID, atendimento.Id);
        v1.AplicarCriacao(
            new DateOnly(2026, 6, 15), 0, "Info extra v1",
            new DescricaoBasica(paciente.Nome, paciente.CPF,
                DateTime.Today.Year - paciente.Nascimento.Year,
                "Engenheiro", "Católico", "Cefaleia", "Corrida"),
            new AGO("13", "01/06/2026", "G1P0", "Sim", StatusVacinaHPV.UmaDose, "Normal", "Nenhum",
                "", "", "", "", "", "", ""),
            new Antecedentes("", "", "", "", "", ""),
            new AntecedentesFamiliares("", ""),
            null,
            new List<ProntuarioAcaoCD>
            {
                new(Guid.Empty, AcoesCD.PedidoExame),
                new(Guid.Empty, AcoesCD.PedidoInternacao)
            },
            new List<Exame>
            {
                new(Guid.Empty, "HEMO", "Hemograma"),
                new(Guid.Empty, "GLIC", "Glicemia")
            },
            new Internacao(Guid.Empty,
                new DateOnly(2026, 6, 20),
                "Cirurgia eletiva",
                "Paciente estável",
                cid.Codigo,
                "30 dias",
                5,
                "Eletiva",
                "Enfermaria",
                "Eletivo",
                false,
                "Hospital Central"));
        await prontuarioRepo.AddAsync(v1);

        // Verify v1 persisted
        var v1Fetched = await prontuarioRepo.GetByIdAsync(v1.ID);
        Assert.NotNull(v1Fetched);
        Assert.Equal(1, v1Fetched!.Versao);
        Assert.Null(v1Fetched.ProntuarioAnteriorId);
        Assert.Equal(2, v1Fetched.Exames.Count);
        Assert.Equal(2, v1Fetched.AcoesCD.Count);
        Assert.NotNull(v1Fetched.Internacao);
        Assert.Equal(cid.Codigo, v1Fetched.Internacao!.CIDCodigo);

        // ── Correction (PUT semantics) ──
        v1Fetched.AplicarCorrecao("Info extra corrigida", "Médico", "Espírita", "Natação");
        await prontuarioRepo.UpdateAsync(v1Fetched);

        var v1Corrected = await prontuarioRepo.GetByIdAsync(v1.ID);
        Assert.NotNull(v1Corrected);
        Assert.Equal(1, v1Corrected!.Versao); // Same versao
        Assert.Equal("Info extra corrigida", v1Corrected.InformacoesExtras);
        Assert.Equal("Médico", v1Corrected.DescricaoBasica.Profissao);
        // Exames/CD unchanged by correction
        Assert.Equal(2, v1Corrected.Exames.Count);

        // ── Clinical evolution (POST /versoes) ──
        var maxVersao = await prontuarioRepo.GetMaxVersaoForPacienteAsync(paciente.ID);
        Assert.Equal(1, maxVersao);

        var v2 = v1Corrected.CriarNovaVersao(
            maxVersao + 1,
            new DateOnly(2026, 7, 15), 1, "Info extra v2",
            new DescricaoBasica(paciente.Nome, paciente.CPF,
                DateTime.Today.Year - paciente.Nascimento.Year,
                "Engenheiro", "Católico", "Retorno - melhora", "Corrida"),
            new AGO("13", "01/07/2026", "G1P0", "Sim", StatusVacinaHPV.UmaDose, "Normal", "Nenhum",
                "", "", "", "", "", "", ""),
            new Antecedentes("", "", "", "", "", ""),
            new AntecedentesFamiliares("", ""),
            null,
            null,
            new List<Exame> { new(Guid.Empty, "TSH", "TSH") },
            null); // No Internacao on v2
        await prontuarioRepo.AddAsync(v2);

        // V1 remains unchanged (except correction fields)
        var v1Final = await prontuarioRepo.GetByIdAsync(v1.ID);
        Assert.NotNull(v1Final);
        Assert.Equal(1, v1Final!.Versao);
        Assert.Equal(2, v1Final.Exames.Count);
        Assert.NotNull(v1Final.Internacao); // Internacao still on v1

        // V2 has new id, incremented versao, chain pointer
        Assert.NotEqual(v1.ID, v2.ID);
        Assert.Equal(2, v2.Versao);
        Assert.Equal(v1.ID, v2.ProntuarioAnteriorId);
        Assert.Single(v2.Exames);
        Assert.Equal("TSH", v2.Exames.First().Codigo);
        Assert.Null(v2.Internacao); // Internacao removed on v2
        Assert.Equal("Retorno - melhora", v2.DescricaoBasica.QD);

        // List by paciente — Versao DESC
        var versions = await prontuarioRepo.GetByPacienteIdAsync(paciente.ID);
        Assert.Equal(2, versions.Count);
        Assert.Equal(2, versions[0].Versao);
        Assert.Equal(1, versions[1].Versao);

        // Latest by paciente
        var latestPaciente = await prontuarioRepo.GetLatestByPacienteIdAsync(paciente.ID);
        Assert.NotNull(latestPaciente);
        Assert.Equal(v2.ID, latestPaciente!.ID);

        // Latest by atendimento
        var latestAtend = await prontuarioRepo.GetLatestByAtendimentoIdAsync(atendimento.Id);
        Assert.NotNull(latestAtend);
        Assert.Equal(v2.ID, latestAtend!.ID);

        // MaxVersao
        maxVersao = await prontuarioRepo.GetMaxVersaoForPacienteAsync(paciente.ID);
        Assert.Equal(2, maxVersao);

        // ── Soft delete v1 ──
        v1Final.MarcarComoExcluido();
        await prontuarioRepo.DeleteAsync(v1Final);

        Assert.Null(await prontuarioRepo.GetByIdAsync(v1.ID));

        // V2 still visible
        var v2AfterDelete = await prontuarioRepo.GetByIdAsync(v2.ID);
        Assert.NotNull(v2AfterDelete);

        versions = await prontuarioRepo.GetByPacienteIdAsync(paciente.ID);
        Assert.Single(versions);
        Assert.Equal(v2.ID, versions[0].ID);

        // MaxVersao still returns 2 (soft-deleted v1 occupies the slot)
        maxVersao = await prontuarioRepo.GetMaxVersaoForPacienteAsync(paciente.ID);
        Assert.Equal(2, maxVersao);

        // ── D-05: V2 collections are complete snapshot, not merge ──
        Assert.Single(v2.Exames);
        Assert.Equal("TSH", v2.Exames.First().Codigo);
        Assert.DoesNotContain(v2.Exames, e => e.Codigo == "HEMO");
        Assert.DoesNotContain(v2.Exames, e => e.Codigo == "GLIC");
    }
}