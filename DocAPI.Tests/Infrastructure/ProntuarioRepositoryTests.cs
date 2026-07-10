using DocAPI.Core.Entities;
using DocAPI.Infrastructure.Repositories;
using DocAPI.Infrastructure.SqlDb.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DocAPI.Tests.Infrastructure;

public class ProntuarioRepositoryTests
{
    private static DocDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<DocDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new DocDbContext(options);
    }

    private static Paciente SamplePaciente(string cpf = "11122233344") =>
        new("Joana Teste", new DateOnly(1985, 3, 20), cpf, "joana@test.com", "31111111111");

    private static Atendimento SampleAtendimento(Guid pacienteId) =>
        new(pacienteId);

    private static Prontuario SampleProntuario(Guid pacienteId, Guid atendimentoId, int tipo = 0,
        string? informacoesExtras = null)
    {
        var p = new Prontuario(pacienteId, atendimentoId);
        p.AplicarCriacao(
            new DateOnly(2026, 6, 1),
            tipo,
            informacoesExtras,
            new DescricaoBasica("Joana Teste", "11122233344", 41, "Engenheira", "Católica", "Dor abdominal", "Corrida"),
            new AGO("12", "01/06/2026", "G2P1", "Sim", StatusVacinaHPV.DuasDoses, "Normal", "Nenhum"),
            new Antecedentes("HAS", "Losartana", "Nenhuma", "Apendicectomia", "Penicilina", "Nenhum"),
            new AntecedentesFamiliares("Mama", "Diabetes"),
            null,
            null,
            null,
            null);
        return p;
    }

    [Fact]
    public async Task AddAsync_ThenGetById_ReturnsProntuario()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new ProntuarioRepository(context);

        var paciente = SamplePaciente();
        context.Pacientes.Add(paciente);
        var atendimento = SampleAtendimento(paciente.ID);
        context.Atendimentos.Add(atendimento);
        await context.SaveChangesAsync();

        var prontuario = SampleProntuario(paciente.ID, atendimento.Id);
        await repo.AddAsync(prontuario);

        var fetched = await repo.GetByIdAsync(prontuario.ID);

        Assert.NotNull(fetched);
        Assert.Equal(prontuario.ID, fetched!.ID);
        Assert.Equal(1, fetched.Versao);
        Assert.Null(fetched.ProntuarioAnteriorId);
        Assert.Equal(paciente.ID, fetched.PacienteId);
        Assert.Equal(atendimento.Id, fetched.AtendimentoId);
        Assert.False(fetched.Deletado);
    }

    [Fact]
    public async Task AplicarCorrecao_ThenUpdate_PreservesIdAndVersao()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new ProntuarioRepository(context);

        var paciente = SamplePaciente();
        context.Pacientes.Add(paciente);
        var atendimento = SampleAtendimento(paciente.ID);
        context.Atendimentos.Add(atendimento);
        await context.SaveChangesAsync();

        var prontuario = SampleProntuario(paciente.ID, atendimento.Id);
        await repo.AddAsync(prontuario);

        prontuario.AplicarCorrecao("Nota administrativa atualizada", "Médica", "Espírita", "Natação");
        await repo.UpdateAsync(prontuario);

        var updated = await repo.GetByIdAsync(prontuario.ID);
        Assert.NotNull(updated);
        Assert.Equal(prontuario.ID, updated!.ID);
        Assert.Equal(1, updated.Versao);
        Assert.Equal("Nota administrativa atualizada", updated.InformacoesExtras);
        Assert.Equal("Médica", updated.DescricaoBasica.Profissao);
        Assert.Equal("Espírita", updated.DescricaoBasica.Religiao);
        Assert.Equal("Natação", updated.DescricaoBasica.AtividadeFisica);
        // Identity fields unchanged
        Assert.Equal("Joana Teste", updated.DescricaoBasica.NomePaciente);
        Assert.Equal("Dor abdominal", updated.DescricaoBasica.QD);
        Assert.NotNull(updated.AtualizadoEm);
    }

    [Fact]
    public async Task CriarNovaVersao_IncrementsVersaoAndKeepsSourceIntact()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new ProntuarioRepository(context);

        var paciente = SamplePaciente();
        context.Pacientes.Add(paciente);
        var atendimento = SampleAtendimento(paciente.ID);
        context.Atendimentos.Add(atendimento);
        await context.SaveChangesAsync();

        var v1 = SampleProntuario(paciente.ID, atendimento.Id);
        await repo.AddAsync(v1);

        var maxVersao = await repo.GetMaxVersaoForPacienteAsync(paciente.ID);
        Assert.Equal(1, maxVersao);

        var nextVersao = maxVersao + 1;
        var v2 = v1.CriarNovaVersao(
            nextVersao,
            new DateOnly(2026, 7, 1),
            1,
            "Evolução clínica",
            new DescricaoBasica("Joana Teste", "11122233344", 42, "Engenheira", "Católica", "Retorno", "Corrida"),
            new AGO("12", "01/07/2026", "G2P1", "Sim", StatusVacinaHPV.DuasDoses, "Normal", "Nenhum"),
            new Antecedentes("HAS", "Losartana", "Nenhuma", "Apendicectomia", "Penicilina", "Nenhum"),
            new AntecedentesFamiliares("Mama", "Diabetes"),
            null, null, null, null);

        await repo.AddAsync(v2);

        // V1 unchanged
        var v1After = await repo.GetByIdAsync(v1.ID);
        Assert.NotNull(v1After);
        Assert.Equal(1, v1After!.Versao);
        Assert.Equal("Dor abdominal", v1After.DescricaoBasica.QD);

        // V2 has new id, incremented versao
        Assert.NotEqual(v1.ID, v2.ID);
        Assert.Equal(2, v2.Versao);
        Assert.Equal(v1.ID, v2.ProntuarioAnteriorId);
        Assert.Equal("Retorno", v2.DescricaoBasica.QD);

        // Both versions listable
        var versions = await repo.GetByPacienteIdAsync(paciente.ID);
        Assert.Equal(2, versions.Count);
        Assert.Equal(2, versions[0].Versao); // DESC
        Assert.Equal(1, versions[1].Versao);

        maxVersao = await repo.GetMaxVersaoForPacienteAsync(paciente.ID);
        Assert.Equal(2, maxVersao);
    }

    [Fact]
    public async Task CriarNovaVersao_InvalidNextVersao_Throws()
    {
        var paciente = SamplePaciente();
        var atendimento = SampleAtendimento(paciente.ID);
        var v1 = SampleProntuario(paciente.ID, atendimento.Id);

        Assert.Throws<InvalidOperationException>(() =>
            v1.CriarNovaVersao(0, new DateOnly(), 0, null,
                new DescricaoBasica("", "", 0, "", "", "", null),
                new AGO("", "", "", "", StatusVacinaHPV.SemInfo, "", ""),
                new Antecedentes("", "", "", "", "", ""),
                new AntecedentesFamiliares("", ""),
                null, null, null, null));
    }

    [Fact]
    public async Task SoftDelete_ExcludesFromReads_ButKeepsInMaxVersao()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new ProntuarioRepository(context);

        var paciente = SamplePaciente();
        context.Pacientes.Add(paciente);
        var atendimento = SampleAtendimento(paciente.ID);
        context.Atendimentos.Add(atendimento);
        await context.SaveChangesAsync();

        var v1 = SampleProntuario(paciente.ID, atendimento.Id, 0, "V1");
        await repo.AddAsync(v1);

        var v2 = v1.CriarNovaVersao(2, new DateOnly(2026, 7, 1), 1, "V2",
            new DescricaoBasica("Joana Teste", "11122233344", 42, "Eng", "Cat", "QD2", null),
            new AGO("", "", "", "", StatusVacinaHPV.SemInfo, "", ""),
            new Antecedentes("", "", "", "", "", ""),
            new AntecedentesFamiliares("", ""),
            null, null, null, null);
        await repo.AddAsync(v2);

        v1.MarcarComoExcluido();
        await repo.DeleteAsync(v1);

        // Deleted version hidden
        Assert.Null(await repo.GetByIdAsync(v1.ID));

        // Non-deleted still visible
        var v2Fetched = await repo.GetByIdAsync(v2.ID);
        Assert.NotNull(v2Fetched);

        // List returns only non-deleted
        var versions = await repo.GetByPacienteIdAsync(paciente.ID);
        Assert.Single(versions);
        Assert.Equal(v2.ID, versions[0].ID);

        // MaxVersao still returns the occupied slot (2) even with soft deleted v1
        var maxVersao = await repo.GetMaxVersaoForPacienteAsync(paciente.ID);
        Assert.Equal(2, maxVersao);
    }

    [Fact]
    public async Task GetLatestByPaciente_ReturnsHighestVersao()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new ProntuarioRepository(context);

        var paciente = SamplePaciente();
        context.Pacientes.Add(paciente);
        var atendimento = SampleAtendimento(paciente.ID);
        context.Atendimentos.Add(atendimento);
        await context.SaveChangesAsync();

        var v1 = SampleProntuario(paciente.ID, atendimento.Id, 0, "V1");
        await repo.AddAsync(v1);

        var v2 = v1.CriarNovaVersao(2, new DateOnly(2026, 7, 1), 1, "V2",
            new DescricaoBasica("Joana Teste", "11122233344", 42, "Eng", "Cat", "QD2", null),
            new AGO("", "", "", "", StatusVacinaHPV.SemInfo, "", ""),
            new Antecedentes("", "", "", "", "", ""),
            new AntecedentesFamiliares("", ""),
            null, null, null, null);
        await repo.AddAsync(v2);

        var latest = await repo.GetLatestByPacienteIdAsync(paciente.ID);
        Assert.NotNull(latest);
        Assert.Equal(v2.ID, latest!.ID);
        Assert.Equal(2, latest.Versao);
    }

    [Fact]
    public async Task GetLatestByAtendimento_ReturnsHighestVersaoForAtendimento()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new ProntuarioRepository(context);

        var paciente = SamplePaciente();
        context.Pacientes.Add(paciente);
        var atendimento = SampleAtendimento(paciente.ID);
        context.Atendimentos.Add(atendimento);
        await context.SaveChangesAsync();

        var v1 = SampleProntuario(paciente.ID, atendimento.Id);
        await repo.AddAsync(v1);

        var latest = await repo.GetLatestByAtendimentoIdAsync(atendimento.Id);
        Assert.NotNull(latest);
        Assert.Equal(v1.ID, latest!.ID);
    }

    [Fact]
    public async Task GetMaxVersaoForPaciente_ReturnsZeroWhenNone()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new ProntuarioRepository(context);

        var max = await repo.GetMaxVersaoForPacienteAsync(Guid.NewGuid());
        Assert.Equal(0, max);
    }

    [Fact]
    public async Task D05_EvolutionDoesNotMergeCollections()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new ProntuarioRepository(context);

        var paciente = SamplePaciente();
        context.Pacientes.Add(paciente);
        var atendimento = SampleAtendimento(paciente.ID);
        context.Atendimentos.Add(atendimento);
        await context.SaveChangesAsync();

        // V1 with E1, E2 + CD1
        var v1 = new Prontuario(paciente.ID, atendimento.Id);
        v1.AplicarCriacao(
            new DateOnly(2026, 6, 1), 0, null,
            new DescricaoBasica("Teste", "11122233344", 30, "Eng", "Cat", "QD", null),
            new AGO("", "", "", "", StatusVacinaHPV.SemInfo, "", ""),
            new Antecedentes("", "", "", "", "", ""),
            new AntecedentesFamiliares("", ""),
            null,
            new List<ProntuarioAcaoCD> { new(Guid.Empty, AcoesCD.PedidoExame) },
            new List<Exame> {
                new(Guid.Empty, "E1", "Exame 1"),
                new(Guid.Empty, "E2", "Exame 2")
            },
            null);
        await repo.AddAsync(v1);

        // V2 evolution payload only supplies E3
        var v2 = v1.CriarNovaVersao(
            2, new DateOnly(2026, 7, 1), 1, null,
            new DescricaoBasica("Teste", "11122233344", 30, "Eng", "Cat", "QD2", null),
            new AGO("", "", "", "", StatusVacinaHPV.SemInfo, "", ""),
            new Antecedentes("", "", "", "", "", ""),
            new AntecedentesFamiliares("", ""),
            null,
            null,
            new List<Exame> { new(Guid.Empty, "E3", "Exame 3") },
            null);
        await repo.AddAsync(v2);

        // V1 remains unchanged with E1, E2
        var v1Fetched = await repo.GetByIdAsync(v1.ID);
        Assert.NotNull(v1Fetched);
        Assert.Equal(2, v1Fetched!.Exames.Count);
        Assert.Contains(v1Fetched.Exames, e => e.Codigo == "E1");
        Assert.Contains(v1Fetched.Exames, e => e.Codigo == "E2");

        // V2 must NOT contain E1, E2 — only E3 (replace, not merge)
        var v2Fetched = await repo.GetByIdAsync(v2.ID);
        Assert.NotNull(v2Fetched);
        Assert.Single(v2Fetched!.Exames);
        Assert.Equal("E3", v2Fetched.Exames.First().Codigo);
    }

    [Fact]
    public async Task GetAll_ReturnsOrderedByVersaoDesc()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new ProntuarioRepository(context);

        var paciente = SamplePaciente("11122233344");
        context.Pacientes.Add(paciente);
        var atendimento = SampleAtendimento(paciente.ID);
        context.Atendimentos.Add(atendimento);
        await context.SaveChangesAsync();

        var v1 = SampleProntuario(paciente.ID, atendimento.Id, 0, "V1");
        await repo.AddAsync(v1);
        var v2 = v1.CriarNovaVersao(2, new DateOnly(2026, 7, 1), 1, "V2",
            new DescricaoBasica("Teste", "11122233344", 42, "Eng", "Cat", "QD2", null),
            new AGO("", "", "", "", StatusVacinaHPV.SemInfo, "", ""),
            new Antecedentes("", "", "", "", "", ""),
            new AntecedentesFamiliares("", ""), null, null, null, null);
        await repo.AddAsync(v2);

        var all = (await repo.GetAllAsync()).ToList();
        Assert.Equal(2, all.Count);
        Assert.Equal(2, all[0].Versao);
        Assert.Equal(1, all[1].Versao);
    }

    [Fact]
    public async Task GetById_NotFound_ReturnsNull()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new ProntuarioRepository(context);

        Assert.Null(await repo.GetByIdAsync(Guid.NewGuid()));
    }
}