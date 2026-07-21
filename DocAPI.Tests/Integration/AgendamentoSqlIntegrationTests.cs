using DocAPI.Core.Entities;
using DocAPI.Infrastructure.Repositories;
using DocAPI.Infrastructure.SqlDb;
using DocAPI.Infrastructure.SqlDb.Context;
using DocAPI.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DocAPI.Tests.Integration;

public class AgendamentoSqlIntegrationTests
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

    private static Paciente BuildSyntheticPaciente(string suffix)
    {
        var paciente = new Paciente(
            $"Paciente Agend Teste {suffix}",
            new DateOnly(1990, 1, 15),
            GenerateSyntheticCpf(),
            $"agend.{suffix}@example.com",
            "31990003333");
        paciente.ComplementarCadastro(
            $"MG{suffix}",
            "Plano Teste",
            $"CART{suffix}",
            new Endereco
            {
                Logradouro = "Rua Agendamento",
                Numero = "200",
                Bairro = "Teste",
                Cidade = "Belo Horizonte",
                UF = "MG",
                CEP = "30130000"
            });
        return paciente;
    }

    private static CID BuildSyntheticCid(string codigo = "Z99.9")
    {
        return new CID { Codigo = codigo, Descricao = "CID sintético para teste de integração Agendamento" };
    }

    [SkippableFact]
    public async Task AgendamentoSql_FullCrudRoundTrip_WithFKValidation()
    {
        var skipReason = await SqlIntegrationTestGate.GetSkipReasonAsync();
        Skip.If(skipReason is not null, skipReason!);

        var connectionString = SqlConnectionResolver.ResolveConnectionString()!;
        await using var context = CreateSqlContext(connectionString);
        var pacienteRepo = new PacienteRepository(context);
        var atendimentoRepo = new AtendimentoRepository(context);
        var prontuarioRepo = new ProntuarioRepository(context);
        var agendamentoRepo = new AgendamentoRepository(context);

        // Seed synthetic CID
        var cid = BuildSyntheticCid();
        if (!await context.CIDs.AnyAsync(c => c.Codigo == cid.Codigo))
        {
            context.CIDs.Add(cid);
            await context.SaveChangesAsync();
        }

        // ── Fixture Chain Step 1: Create Paciente ──
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var paciente = BuildSyntheticPaciente(suffix);
        await pacienteRepo.CreateAsync(paciente);

        // ── Fixture Chain Step 2: Create Atendimento ──
        var atendimento = new Atendimento(paciente.ID, "Integração agendamento");
        await atendimentoRepo.CreateAsync(atendimento);

        // ── Fixture Chain Step 3: Create Prontuario with Internacao ──
        var prontuario = new Prontuario(paciente.ID, atendimento.Id);
        prontuario.AplicarCriacao(
            new DateOnly(2026, 7, 10), 0, "Prontuário para teste de agendamento",
            new DescricaoBasica(paciente.Nome, paciente.CPF,
                DateTime.Today.Year - paciente.Nascimento.Year,
                "Engenheiro", "Católico", "Cefaleia", "Corrida"),
            new AGO("13", "01/07/2026", "G1P0", "Sim", StatusVacinaHPV.UmaDose, "Normal", "Nenhum",
                "", "", "", "", "", "", ""),
            new Antecedentes("", "", "", "", "", ""),
            new AntecedentesFamiliares("", ""),
            null,
            new List<ProntuarioAcaoCD> { new(Guid.Empty, AcoesCD.PedidoInternacao) },
            null,
            new Internacao(Guid.Empty,
                new DateOnly(2026, 7, 10),
                "Cirurgia eletiva",
                "Paciente estável para procedimento",
                cid.Codigo,
                "30 dias",
                3,
                "Eletiva",
                "Enfermaria",
                "Eletivo",
                false,
                "Hospital Central"));
        await prontuarioRepo.AddAsync(prontuario);

        // Retrieve the Internacao from the persisted Prontuario
        var prontuarioFetched = await context.Prontuarios
            .Include(p => p.Internacao)
            .FirstOrDefaultAsync(p => p.ID == prontuario.ID);
        Assert.NotNull(prontuarioFetched);
        Assert.NotNull(prontuarioFetched!.Internacao);
        var internacaoId = prontuarioFetched.Internacao!.ID;

        // ── Fixture Chain Step 4: CRUD Round-trip ──

        // CREATE — with PacienteID
        var agendamento = new Agendamento(
            internacaoId: internacaoId,
            atendimentoId: atendimento.Id,
            data: new DateOnly(2026, 7, 20),
            horario: new TimeOnly(14, 0),
            pacienteID: paciente.ID,
            nome: "Maria Silva",
            aviso: "Jejum 8h",
            local: "Hospital Central",
            sala: "3A",
            instrucaoStatus: StatusInstrucoes.SemSolicitação,
            atestadoStatus: StatusAtestado.NaoRealizado);

        await agendamentoRepo.CreateAsync(agendamento);

        // READ by ID
        var fetched = await agendamentoRepo.GetByIdAsync(agendamento.ID);
        Assert.NotNull(fetched);
        Assert.Equal(agendamento.ID, fetched!.ID);
        Assert.Equal("Maria Silva", fetched.Nome);
        Assert.Equal(StatusAgendamento.SemSenha, fetched.Status);
        Assert.Equal(paciente.ID, fetched.PacienteID);
        Assert.True(fetched.CriadoEm != default);

        // READ by Name (substring search)
        var byName = await agendamentoRepo.GetByNameAsync("Maria");
        Assert.NotEmpty(byName);
        Assert.Contains(byName, a => a.ID == agendamento.ID);

        // READ by PacienteId
        var byPaciente = await agendamentoRepo.GetByPacienteIdAsync(paciente.ID);
        Assert.NotEmpty(byPaciente);
        Assert.Contains(byPaciente, a => a.ID == agendamento.ID);

        // READ paginated
        var all = await agendamentoRepo.GetAllAsync(0, 10);
        Assert.NotEmpty(all);

        // UPDATE
        fetched.Update(
            nome: "Maria Atualizada",
            aviso: "Jejum 12h",
            local: "Hospital Central Bloco B",
            status: StatusAgendamento.SenhaAprovada);
        fetched.SetSenha(new SenhaAgendamento
        {
            Codigo = "SENHA001",
            DataPedido = new DateOnly(2026, 7, 15)
        });
        await agendamentoRepo.UpdateAsync(fetched, fetched.ID);

        // READ updated
        var updated = await agendamentoRepo.GetByIdAsync(agendamento.ID);
        Assert.NotNull(updated);
        Assert.Equal("Maria Atualizada", updated!.Nome);
        Assert.Equal("Jejum 12h", updated.Aviso);
        Assert.Equal("Hospital Central Bloco B", updated.Local);
        Assert.Equal(StatusAgendamento.SenhaAprovada, updated.Status);
        Assert.NotNull(updated.SenhaAgendamento);
        Assert.Equal("SENHA001", updated.SenhaAgendamento!.Codigo);
        Assert.NotNull(updated.AtualizadoEm);

        // FK immutability after update
        Assert.Equal(atendimento.Id, updated.AtendimentoId);
        Assert.Equal(internacaoId, updated.InternacaoId);
        Assert.Equal(paciente.ID, updated.PacienteID);

        // SOFT DELETE
        await agendamentoRepo.DeleteAsync(agendamento.ID);

        // Verify excluded from reads
        var deleted = await agendamentoRepo.GetByIdAsync(agendamento.ID);
        Assert.Null(deleted);

        var byNameAfterDelete = await agendamentoRepo.GetByNameAsync("Maria");
        Assert.DoesNotContain(byNameAfterDelete, a => a.ID == agendamento.ID);

        // ── CREATE without PacienteID (nullable FK) ──
        var agendamentoSemPaciente = new Agendamento(
            internacaoId: internacaoId,
            atendimentoId: atendimento.Id,
            data: new DateOnly(2026, 7, 25),
            horario: new TimeOnly(10, 0),
            pacienteID: null,
            nome: "Sem Paciente");

        await agendamentoRepo.CreateAsync(agendamentoSemPaciente);
        var fetchedSemPaciente = await agendamentoRepo.GetByIdAsync(agendamentoSemPaciente.ID);
        Assert.NotNull(fetchedSemPaciente);
        Assert.Null(fetchedSemPaciente!.PacienteID);

        // ── Negative FK validation: invalid AtendimentoId ──
        var agendamentoInvalidAtendimento = new Agendamento(
            internacaoId: internacaoId,
            atendimentoId: Guid.NewGuid(),
            data: new DateOnly(2026, 7, 30),
            horario: new TimeOnly(9, 0),
            nome: "FK Teste");

        await Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            await agendamentoRepo.CreateAsync(agendamentoInvalidAtendimento);
        });
    }

    [SkippableFact]
    public async Task AgendamentoSql_Pagination_CapsAt100()
    {
        var skipReason = await SqlIntegrationTestGate.GetSkipReasonAsync();
        Skip.If(skipReason is not null, skipReason!);

        var connectionString = SqlConnectionResolver.ResolveConnectionString()!;
        await using var context = CreateSqlContext(connectionString);
        var agendamentoRepo = new AgendamentoRepository(context);

        var all = await agendamentoRepo.GetAllAsync(0, 200);
        Assert.True(all.Count() <= 100);
    }
}