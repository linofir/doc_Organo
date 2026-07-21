using DocAPI.Core.Entities;
using Xunit;

namespace DocAPI.Tests.Infrastructure;

public class AgendamentoEntityTests
{
    private static Agendamento CreateSample(Guid? pacienteID = null) =>
        new(
            internacaoId: Guid.NewGuid(),
            atendimentoId: Guid.NewGuid(),
            data: new DateOnly(2026, 7, 20),
            horario: new TimeOnly(14, 0),
            pacienteID: pacienteID,
            nome: "Maria Silva",
            aviso: "Jejum 8h",
            local: "Hospital Central",
            sala: "3A",
            instrucaoStatus: StatusInstrucoes.SemSolicitação,
            atestadoStatus: StatusAtestado.NaoRealizado);

    [Fact]
    public void Factory_SetsRequiredProperties()
    {
        var a = CreateSample();

        Assert.NotEqual(Guid.Empty, a.ID);
        Assert.Equal(StatusAgendamento.SemSenha, a.Status);
        Assert.NotEqual(default, a.CriadoEm);
        Assert.True(a.CriadoEm <= DateTime.UtcNow);
        Assert.False(a.Deletado);
        Assert.Null(a.DeletadoEm);
        Assert.Null(a.PacienteID);
    }

    [Fact]
    public void Factory_AcceptsOptionalPacienteID()
    {
        var pacienteID = Guid.NewGuid();
        var a = CreateSample(pacienteID);

        Assert.Equal(pacienteID, a.PacienteID);
    }

    [Fact]
    public void SoftDelete_SetsDeletionMarkers()
    {
        var a = CreateSample();

        a.SoftDelete();

        Assert.True(a.Deletado);
        Assert.NotNull(a.DeletadoEm);
        Assert.True(a.DeletadoEm <= DateTime.UtcNow);
    }

    [Fact]
    public void Update_ChangesMutableFields()
    {
        var a = CreateSample();

        a.Update(
            nome: "Novo Nome",
            aviso: "Novo aviso",
            status: StatusAgendamento.SenhaAprovada,
            instrucaoStatus: StatusInstrucoes.Concluido,
            atestadoStatus: StatusAtestado.Realizado,
            dataConsulta: new DateOnly(2026, 8, 1));

        Assert.Equal("Novo Nome", a.Nome);
        Assert.Equal("Novo aviso", a.Aviso);
        Assert.Equal(StatusAgendamento.SenhaAprovada, a.Status);
        Assert.Equal(StatusInstrucoes.Concluido, a.InstrucaoStatus);
        Assert.Equal(StatusAtestado.Realizado, a.AtestadoStatus);
        Assert.Equal(new DateOnly(2026, 8, 1), a.DataConsulta);
        Assert.NotNull(a.AtualizadoEm);
    }

    [Fact]
    public void Update_DoesNotChangeFKs()
    {
        var a = CreateSample();
        var originalAtendimentoId = a.AtendimentoId;
        var originalInternacaoId = a.InternacaoId;
        var originalID = a.ID;

        a.Update(nome: "Novo Nome");

        Assert.Equal(originalAtendimentoId, a.AtendimentoId);
        Assert.Equal(originalInternacaoId, a.InternacaoId);
        Assert.Equal(originalID, a.ID);
    }

    [Fact]
    public void Update_PartialFieldsDoNotAffectUnspecified()
    {
        var a = CreateSample();
        var originalHorario = a.Horario;

        a.Update(nome: "Apenas nome");

        Assert.Equal("Apenas nome", a.Nome);
        Assert.Equal(originalHorario, a.Horario);
    }

    [Fact]
    public void SetSenha_SetsAndReplacesValue()
    {
        var a = CreateSample();
        var senha = new SenhaAgendamento
        {
            Codigo = "ABC123",
            DataPedido = new DateOnly(2026, 7, 1)
        };

        a.SetSenha(senha);

        Assert.NotNull(a.SenhaAgendamento);
        Assert.Equal("ABC123", a.SenhaAgendamento!.Codigo);
        Assert.NotNull(a.AtualizadoEm);
    }

    [Fact]
    public void SetSenha_ClearsWithNull()
    {
        var a = CreateSample();
        a.SetSenha(new SenhaAgendamento { Codigo = "ABC123" });

        a.SetSenha(null);

        Assert.Null(a.SenhaAgendamento);
    }
}