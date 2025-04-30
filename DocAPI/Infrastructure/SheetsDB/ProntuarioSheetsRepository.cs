using DocAPI.Core.Models;
using DocAPI.Core.Repositories;
using DocAPI.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace DocAPI.Infrastructure.Sheets;

public class ProntuarioSheetsRepository : IProntuarioRepository
{
    private readonly GoogleSheetsDB _sheetsDB;
    public ProntuarioSheetsRepository(GoogleSheetsDB sheets)
    {
        _sheetsDB = sheets;
    }
    public async Task<IEnumerable<Paciente>> GetAllAsync(int skip = 0, int take = 10)
    {
        var pacientes = await GetPacientesAsync();
        return pacientes.Skip(skip).Take(take);
    }
    public async Task<Paciente?> GetByIdAsync(string id)
    {
        var pacientes = await GetPacientesAsync();
        return pacientes.FirstOrDefault(p => p.ID == id);
    }

    public async Task CreateAsync(Paciente paciente)
    {
        await AddPacienteAsync( paciente);
    }

    public async Task UpdateAsync(Paciente paciente, string id)
    {
        await UpdatePacienteAsync(paciente, id);
    }

    public async Task DeleteAsync(string id)
    {
        await DeletePacienteAsync(id);
    }
    
    public async Task<List<Paciente>> GetPacientesAsync()
    {
        var values = await _sheetsDB.LerRangeAsync("Pacientes!A3:O"); // de A até a coluna ID
        var pacientes = new List<Paciente>();
        var limit = values.Count;
        Console.WriteLine($"Total de linhas com algum dado: {limit}");       
        for (int i = 0; i < limit; i++)
        {
            var row = values[i];
            if (row.All(cell => string.IsNullOrWhiteSpace(cell?.ToString()))) continue;
            //Confere se tem alguma coluna vazia
            if (row.Count < 15)
            {
                Console.WriteLine($"Linha {i + 3} ignorada: colunas insuficientes ({row.Count}).");
                continue;
            }
            try
            {
                var nascimentoString = row[2]?.ToString();
                DateTime nascimento = DateTime.MinValue;

                if (!string.IsNullOrWhiteSpace(nascimentoString))
                {
                    DateTime.TryParse(nascimentoString, out nascimento);
                }

                var paciente = new Paciente
                {
                    CPF = row[0]?.ToString(),
                    Nome = row[1]?.ToString(),
                    Nascimento = nascimento,
                    Plano = row[3]?.ToString(),
                    ID = row[4]?.ToString(),
                    Carteira = row[5]?.ToString(),
                    Email = row[6]?.ToString(),
                    Telefone = row[7]?.ToString(),
                    Endereco = new Endereco
                    {
                        Logradouro = row[8]?.ToString(),
                        Numero = row[9]?.ToString(),
                        Bairro = row[10]?.ToString(),
                        Cidade = row[11]?.ToString(),
                        UF = row[12]?.ToString(),
                        CEP = row[13]?.ToString()
                    },
                    RG = row.Count > 14 ? row[14]?.ToString() : null
                };
                pacientes.Add(paciente);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar linha {i + 3}: {ex.Message}");
            }

        }

        return pacientes;
    }
    public async Task AddPacienteAsync(Paciente paciente)
    {
        // 1. Ler as linhas existentes
        var valores = await _sheetsDB.LerRangeAsync("Pacientes!A3:O");
        int novaLinhaIndex = valores.Count + 3; // +3 porque a planilha começa na linha 3
        // 2. Preparar os valores a serem inseridos
        ValueRange body = new ValueRange
        {
            Values = new List<IList<object>> {
                new List<object> {
                    paciente.CPF,
                    paciente.Nome,
                    paciente.Nascimento.ToString("dd/MM/yyyy"),
                    paciente.Plano,
                    paciente.ID,
                    paciente.Carteira,
                    paciente.Email,
                    paciente.Telefone,
                    paciente.Endereco?.Logradouro,
                    paciente.Endereco?.Numero,
                    paciente.Endereco?.Bairro,
                    paciente.Endereco?.Cidade,
                    paciente.Endereco?.UF,
                    paciente.Endereco?.CEP,
                    paciente.RG
                }
            }
        };


        // 3. Escrever os dados na próxima linha disponível
        string rangeDestino = $"Pacientes!A{novaLinhaIndex}:O{novaLinhaIndex}";
        Console.WriteLine($"A nova Paciente será acrescentada na { rangeDestino}");
        await _sheetsDB.WriteRangeAsync(rangeDestino, body.Values);
    }
    public async Task UpdatePacienteAsync(Paciente paciente, string id)
    {
        // Passo 1: Buscar a linha do paciente (por ID)
        var allPacientes = await GetPacientesAsync();
        int linhaIndex = allPacientes.FindIndex(p => p.ID == id);
        //int linhaIndex = allPacientes.FindIndex(p => p.ID == paciente.ID);

        if (linhaIndex == -1)
            throw new Exception("Paciente não encontrado na planilha.");

        // Passo 2: A linha no Google Sheets começa em 2 (1 para header)
        int linhaNoSheet = linhaIndex + 3;

        // Passo 3: Criar os dados atualizados
        var valoresAtualizados = new List<IList<object?>>
        {
            new List<object?>
            {
                paciente.CPF,
                paciente.Nome,
                paciente.Nascimento.ToString("dd/MM/yyyy"),
                paciente.Plano,
                paciente.ID,
                paciente.Carteira,
                paciente.Email,
                paciente.Telefone,
                paciente.Endereco?.Logradouro,
                paciente.Endereco?.Numero,
                paciente.Endereco?.Bairro,
                paciente.Endereco?.Cidade,
                paciente.Endereco?.UF,
                paciente.Endereco?.CEP,
                paciente.RG
            }
        };

        // 5. Montar o range da linha específica (A até O)
        string range = $"Pacientes!A{linhaNoSheet}:O{linhaNoSheet}";

        Console.WriteLine($"Atualizando paciente com ID '{paciente.ID}' na linha {linhaNoSheet}");

        // 6. Escrever na planilha
        await _sheetsDB.WriteRangeAsync(range, valoresAtualizados);
    }
    public async Task DeletePacienteAsync(string id)
    {

        // Passo 1: Buscar a linha do paciente (por ID)
        var allPacientes = await GetPacientesAsync();
        int linhaIndex = allPacientes.FindIndex(p => p.ID == id);
        //int linhaIndex = allPacientes.FindIndex(p => p.ID == paciente.ID);

        if (linhaIndex == -1)
            throw new Exception("Paciente não encontrado na planilha.");

        // Passo 2: A linha no Google Sheets começa em 2 (1 para header)
        int linhaNoSheet = linhaIndex + 2;
        Console.WriteLine($"A linha deletada será {linhaNoSheet}");

        await _sheetsDB.DeleteLineAsync(linhaNoSheet, "Pacientes");
    }
    


}


