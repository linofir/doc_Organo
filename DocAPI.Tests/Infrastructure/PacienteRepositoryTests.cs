using AutoMapper;
using DocAPI.Core.Entities;
using DocAPI.Data.Dtos;
using DocAPI.Infrastructure.Repositories;
using DocAPI.Infrastructure.SqlDb.Context;
using DocAPI.Profiles;
using Microsoft.EntityFrameworkCore;
using Xunit;

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

    private static Paciente SamplePaciente(string cpf = "12345678901", string nome = "Maria Silva") =>
        new(nome, new DateOnly(1990, 5, 15), cpf, "maria@example.com", "11999999999");

    private static Paciente FullSamplePaciente(string cpf = "12345678901", string nome = "Maria Silva")
    {
        var paciente = SamplePaciente(cpf, nome);
        paciente.ComplementarCadastro(
            "MG1234567",
            "Unimed",
            "CART001",
            new Endereco
            {
                Logradouro = "Rua Teste",
                Numero = "100",
                Bairro = "Centro",
                Cidade = "Belo Horizonte",
                UF = "MG",
                CEP = "30130000"
            });
        return paciente;
    }

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
    public async Task CreateAsync_PersistsAllFields()
    {
        await using var context = CreateContext(nameof(CreateAsync_PersistsAllFields));
        var repo = new PacienteRepository(context);
        var paciente = FullSamplePaciente();

        await repo.CreateAsync(paciente);

        var saved = await repo.GetByIdAsync(paciente.ID);
        Assert.NotNull(saved);
        Assert.Equal("MG1234567", saved.RG);
        Assert.Equal("Unimed", saved.Plano);
        Assert.Equal("CART001", saved.Carteira);
        Assert.NotNull(saved.Endereco);
        Assert.Equal("Rua Teste", saved.Endereco!.Logradouro);
        Assert.Equal("100", saved.Endereco.Numero);
        Assert.Equal("Centro", saved.Endereco.Bairro);
        Assert.Equal("Belo Horizonte", saved.Endereco.Cidade);
        Assert.Equal("MG", saved.Endereco.UF);
        Assert.Equal("30130000", saved.Endereco.CEP);
    }

    [Fact]
    public async Task UpdateAsync_PersistsAllFields()
    {
        await using var context = CreateContext(nameof(UpdateAsync_PersistsAllFields));
        var repo = new PacienteRepository(context);
        var paciente = FullSamplePaciente();
        await repo.CreateAsync(paciente);

        var updated = FullSamplePaciente(paciente.CPF, "Maria Santos");
        updated.ComplementarCadastro(
            "MG7654321",
            "Bradesco",
            "CART002",
            new Endereco
            {
                Logradouro = "Av Atualizada",
                Numero = "200",
                Bairro = "Savassi",
                Cidade = "Belo Horizonte",
                UF = "MG",
                CEP = "30140000"
            });

        await repo.UpdateAsync(updated, paciente.ID);

        var found = await repo.GetByIdAsync(paciente.ID);
        Assert.NotNull(found);
        Assert.Equal("Maria Santos", found.Nome);
        Assert.Equal("MG7654321", found.RG);
        Assert.Equal("Bradesco", found.Plano);
        Assert.Equal("CART002", found.Carteira);
        Assert.Equal("Av Atualizada", found.Endereco!.Logradouro);
        Assert.Equal("200", found.Endereco.Numero);
        Assert.Equal("Savassi", found.Endereco.Bairro);
        Assert.Equal("30140000", found.Endereco.CEP);
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
    public async Task GetPacienteByCpfAsync_UsesExactMatch()
    {
        await using var context = CreateContext(nameof(GetPacienteByCpfAsync_UsesExactMatch));
        var repo = new PacienteRepository(context);
        await repo.CreateAsync(SamplePaciente("11122233344"));

        var results = await repo.GetPacienteByCpfAsync("111222333");

        Assert.Empty(results);
    }

    [Fact]
    public async Task GetPacienteByNomeAsync_ReturnsPartialMatches()
    {
        await using var context = CreateContext(nameof(GetPacienteByNomeAsync_ReturnsPartialMatches));
        var repo = new PacienteRepository(context);
        await repo.CreateAsync(SamplePaciente("11111111111", "Ana Costa"));
        await repo.CreateAsync(SamplePaciente("22222222222", "Ana Paula"));
        await repo.CreateAsync(SamplePaciente("33333333333", "Bruno Lima"));

        var results = await repo.GetPacienteByNomeAsync("Ana");

        Assert.Equal(2, results.Count);
        Assert.All(results, p => Assert.Contains("Ana", p.Nome));
    }

    [Fact]
    public async Task GetAllAsync_OrdersByNomeAndPaginates()
    {
        await using var context = CreateContext(nameof(GetAllAsync_OrdersByNomeAndPaginates));
        var repo = new PacienteRepository(context);
        await repo.CreateAsync(SamplePaciente("44444444444", "Carla Souza"));
        await repo.CreateAsync(SamplePaciente("55555555555", "Beatriz Alves"));
        await repo.CreateAsync(SamplePaciente("66666666666", "Daniela Rocha"));

        var page = (await repo.GetAllAsync(skip: 1, take: 1)).ToList();

        Assert.Single(page);
        Assert.Equal("Carla Souza", page[0].Nome);
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
    public async Task DeleteAsync_ExcludesPatientFromDefaultQueries()
    {
        await using var context = CreateContext(nameof(DeleteAsync_ExcludesPatientFromDefaultQueries));
        var repo = new PacienteRepository(context);
        var paciente = FullSamplePaciente("77777777777", "Elena Ferreira");
        await repo.CreateAsync(paciente);

        await repo.DeleteAsync(paciente.ID);

        Assert.Null(await repo.GetByIdAsync(paciente.ID));
        Assert.Empty(await repo.GetPacienteByCpfAsync("77777777777"));
        Assert.Empty(await repo.GetPacienteByNomeAsync("Elena"));
        Assert.DoesNotContain(
            (await repo.GetAllAsync()).Select(p => p.ID),
            id => id == paciente.ID);
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

public class PacienteMappingTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<PacienteProfile>());
        return config.CreateMapper();
    }

    [Fact]
    public void CreatePacienteDto_MapsAllFields()
    {
        var mapper = CreateMapper();
        var dto = new CreatePacienteDto
        {
            Nome = "Paciente Teste",
            Nascimento = new DateOnly(1985, 3, 10),
            CPF = "10987654321",
            RG = "MG9988776",
            Email = "teste@example.com",
            Telefone = "31988887777",
            Plano = "Amil",
            Carteira = "CART999",
            Endereco = new CreateEnderecoDto
            {
                Logradouro = "Rua Mapa",
                Numero = "50",
                Bairro = "Funcionarios",
                Cidade = "Belo Horizonte",
                UF = "MG",
                CEP = "30110000"
            }
        };

        var entity = mapper.Map<Paciente>(dto);

        Assert.Equal(dto.Nome, entity.Nome);
        Assert.Equal(dto.Nascimento, entity.Nascimento);
        Assert.Equal(dto.CPF, entity.CPF);
        Assert.Equal(dto.RG, entity.RG);
        Assert.Equal(dto.Email, entity.Email);
        Assert.Equal(dto.Telefone, entity.Telefone);
        Assert.Equal(dto.Plano, entity.Plano);
        Assert.Equal(dto.Carteira, entity.Carteira);
        Assert.NotNull(entity.Endereco);
        Assert.Equal("Rua Mapa", entity.Endereco!.Logradouro);
        Assert.Equal("50", entity.Endereco.Numero);
        Assert.Equal("Funcionarios", entity.Endereco.Bairro);
        Assert.Equal("Belo Horizonte", entity.Endereco.Cidade);
        Assert.Equal("MG", entity.Endereco.UF);
        Assert.Equal("30110000", entity.Endereco.CEP);
    }

    [Fact]
    public void UpdatePacienteDto_MapsAllFields()
    {
        var mapper = CreateMapper();
        var dto = new UpdatePacienteDto
        {
            Nome = "Paciente Atualizado",
            Nascimento = new DateOnly(1985, 3, 10),
            CPF = "10987654321",
            RG = "MG1122334",
            Email = "atualizado@example.com",
            Telefone = "31977776666",
            Plano = "SulAmerica",
            Carteira = "CART555",
            Endereco = new UpdateEnderecoDto
            {
                Logradouro = "Av Update",
                Numero = "900",
                Bairro = "Lourdes",
                Cidade = "Belo Horizonte",
                UF = "MG",
                CEP = "30120000"
            }
        };

        var entity = mapper.Map<Paciente>(dto);

        Assert.Equal(dto.Plano, entity.Plano);
        Assert.Equal(dto.Carteira, entity.Carteira);
        Assert.Equal(dto.RG, entity.RG);
        Assert.Equal("Av Update", entity.Endereco!.Logradouro);
        Assert.Equal("900", entity.Endereco.Numero);
    }

    [Fact]
    public void Paciente_MapsToReadPacienteDto()
    {
        var mapper = CreateMapper();
        var paciente = new Paciente(
            "Leitura Teste",
            new DateOnly(1992, 7, 20),
            "55667788990",
            "leitura@example.com",
            "31966665555");
        paciente.ComplementarCadastro("MG4455667", "Particular", "CART777", new Endereco
        {
            Logradouro = "Rua Leitura",
            Numero = "10",
            Bairro = "Centro",
            Cidade = "Contagem",
            UF = "MG",
            CEP = "32010000"
        });

        var dto = mapper.Map<ReadPacienteDto>(paciente);

        Assert.Equal(paciente.ID.ToString(), dto.ID);
        Assert.Equal("Leitura Teste", dto.Nome);
        Assert.Equal("55667788990", dto.CPF);
        Assert.Equal("Particular", dto.Plano);
        Assert.Equal("Rua Leitura", dto.Endereco.Logradouro);
    }
}
