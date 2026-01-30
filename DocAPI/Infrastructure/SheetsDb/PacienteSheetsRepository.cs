using System.ComponentModel.DataAnnotations;
using DocAPI.Core.Models;
using DocAPI.Core.Repositories;
using DocAPI.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace DocAPI.Infrastructure.SheetsDb;

public class PacienteSheetsRepository : IPacienteRepository
{
    private readonly GoogleSheetsDB _sheetsDB;
    // private readonly IProntuarioRepository _IProntuarioRepository;
    // private readonly PdfGeneratorService _pdfGeneratorService;
    public PacienteSheetsRepository(GoogleSheetsDB sheets)//*, IProntuarioRepository prontuarioRepo, PdfGeneratorService pdfGen*//* )
    {
        _sheetsDB = sheets;
        // _IProntuarioRepository = prontuarioRepo;
        // _pdfGeneratorService =  pdfGen;
    }
    public async Task<IEnumerable<Paciente>> GetAllAsync(int skip = 0, int take = 10)
    {
        var pacientes = await GetPacientesAsync();
        return pacientes.Skip(skip).Take(take);
    }
    public async Task<Paciente?> GetByIdAsync(string id)
    {
        //Console.WriteLine("👉 Início de GetByIdAsync");

        List<Paciente> pacientes;
        pacientes = await GetPacientesAsync();
        // try
        // {
        //     Console.WriteLine("✅ GetPacientesAsync retornou com sucesso.");
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine($"❌ Erro em GetPacientesAsync: {ex.Message}");
        //     throw;
        // }
        // Console.WriteLine($"🔍 Buscando paciente com ID: '{id?.Trim()}'");
        // Console.WriteLine("🧾 Lista de IDs disponíveis:");

        // foreach (var p in pacientes)
        // {
        //     Console.WriteLine($"- '{p.ID?.Trim()}'");
        // }

        var paciente = pacientes.FirstOrDefault(p =>
            !string.IsNullOrWhiteSpace(p.ID) &&
            !string.IsNullOrWhiteSpace(id) &&
            p.ID.Trim().Equals(id.Trim(), StringComparison.OrdinalIgnoreCase));

        if (paciente == null)
        {
            Console.WriteLine($"❌ Paciente com ID '{id}' não encontrado.");
        }
        // else
        // {
        //     //Console.WriteLine($"✅ Paciente encontrado: {paciente.Nome}");
        // }

        return paciente;
    }
    public async Task<List<Paciente>> GetPacienteByCpfAsync(string cpf)
    {
        var paciente = await GetAgendamentoByFilterAsync( cpf, PacientesFilter.Cpf);
        // Console.WriteLine($"test inicio metodo, procurando: {cpf}");
        // List<Paciente> pacientes;
        // pacientes = await GetPacientesAsync();
        // Console.WriteLine(pacientes.Count());
        // var paciente = pacientes.FirstOrDefault(p =>
        //     !string.IsNullOrWhiteSpace(p.CPF) &&
        //     !string.IsNullOrWhiteSpace(cpf) &&
        //     p.CPF.Trim().Equals(cpf.Trim(), StringComparison.OrdinalIgnoreCase));
        // throw new NotImplementedException();
        return paciente;
    }
    public async Task<List<Paciente>> GetPacienteByNomeAsync(string nome)
    {
        var paciente = await GetAgendamentoByFilterAsync( nome, PacientesFilter.Nome);
        // Console.WriteLine($"test inicio metodo, procurando: {cpf}");
        // List<Paciente> pacientes;
        // pacientes = await GetPacientesAsync();
        // Console.WriteLine(pacientes.Count());
        // var paciente = pacientes.FirstOrDefault(p =>
        //     !string.IsNullOrWhiteSpace(p.CPF) &&
        //     !string.IsNullOrWhiteSpace(cpf) &&
        //     p.CPF.Trim().Equals(cpf.Trim(), StringComparison.OrdinalIgnoreCase));
        // throw new NotImplementedException();
        return paciente;
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
    //   public async Task<Stream> CreateReportByIdAsync( string pacienteId )
    // {
    //     var paciente = await GetByIdAsync(pacienteId);
    //     if(paciente == null)
    //     {
    //         throw new InvalidOperationException($"Paciente com ID '{pacienteId}' não encontrado.");
    //     }
    //     var prontuariosOfPaciente = await _IProntuarioRepository.GetProntuariosOfPacienteAsync(paciente);
    //     return _pdfGeneratorService.GeneratePatientReportPdf( paciente, prontuariosOfPaciente);
    // }
    // public async Task<Stream> CreateReportByCpfAsync( string pacienteCpf )
    // {
    //     var paciente = await GetPacienteByCpfAsync(pacienteCpf);
    //     if(paciente == null) 
    //     {
    //         throw new InvalidOperationException($"Paciente com CPF '{pacienteCpf}' não encontrado.");
    //     }
    //     var prontuariosOfPaciente = await _IProntuarioRepository.GetProntuariosOfPacienteAsync( paciente[0]);
    //     //Adicionar Coleta de agendamentos da paciente
    //     return _pdfGeneratorService.GeneratePatientReportPdf( paciente[0], prontuariosOfPaciente);
    // }
    public async Task<List<Paciente>> GetPacientesAsync()
    {
        var values = await _sheetsDB.LerRangeAsync("Pacientes!A3:O"); // de A até a coluna ID
        var pacientes = new List<Paciente>();
        var limit = values.Count;
        //Console.WriteLine($"Total de linhas com algum dado: {limit}");       
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
                DateOnly nascimento = DateOnly.MinValue;
                if (!string.IsNullOrWhiteSpace(nascimentoString))
                {
                    DateOnly.TryParse(nascimentoString, out nascimento);
                }
                var paciente = InstantiatePaciente(row);
                pacientes.Add(paciente);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar linha {i + 3}: {ex.Message}");
            }
        }
        return pacientes;
    }
    public async Task<List<Paciente>> GetAgendamentoByFilterAsync(string conditionOfRow, PacientesFilter rowIndex)
    {
        Console.WriteLine($"Filtrando pacientes da coluna(i): {rowIndex } para {conditionOfRow}" );
        var values = await _sheetsDB.LerRangeAsync("Pacientes!A3:O"); // de A até a coluna ID
        var pacientesOfcondition = new List<Paciente>{};
        var limit = values.Count;
        //Console.WriteLine($"Total de linhas com algum dado: {limit}");       
        for (int i = 0; i < limit; i++)
        {
            var row = values[i];
            if (row.All(cell => string.IsNullOrWhiteSpace(cell?.ToString()))) continue;
            //Confere se tem alguma coluna vazia
            if (row.Count < 14)
            {
                Console.WriteLine($"Linha {i + 3} ignorada: colunas insuficientes ({row.Count}).");
                continue;
            }

            try
            {
                string? cellValue = null;
                if ((int)rowIndex < row.Count) // Garante que o índice existe na linha, talvez seja redundante já que uso um Enum para garantir isso.
                {
                    cellValue = row[(int)rowIndex]?.ToString();
                }
                if(!string.IsNullOrWhiteSpace(cellValue) && 
                    conditionOfRow.Trim().Equals(cellValue.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Filtrando agendamento...encontrado");        
                    Paciente paciente = InstantiatePaciente(row);
                    pacientesOfcondition.Add(paciente);
                    Console.WriteLine($"Paciente {paciente.Nome}, foi encontrado");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Erro ao processar linha {i + 3}: {ex.Message}");
            }

        }
        return pacientesOfcondition;
        
    }
    public async Task AddPacienteAsync(Paciente paciente)
    {
        // 1. Ler as linhas existentes
        var pacientesheet = await _sheetsDB.LerRangeAsync("Pacientes!A3:O");
        int novaLinhaIndex = pacientesheet.Count(r => r.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString()))) + 3;
        if (string.IsNullOrEmpty(paciente.ID))
        {
            paciente.ID = Guid.NewGuid().ToString();
        }
        ValueRange body = CreatePacienteToSheet(paciente);
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
                id,
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

        Console.WriteLine($"Atualizando paciente com ID '{id}' na linha {linhaNoSheet}");

        // 6. Escrever na planilha
        await _sheetsDB.WriteRangeAsync(range, valoresAtualizados);
    }
    public async Task DeletePacienteAsync(string id)
    {
        // Passo 1: Buscar a linha do paciente (por ID)
        // var allPacientes = await GetPacientesAsync();
        // int linhaIndex = allPacientes.FindIndex(p => p.ID == id);
        //int linhaIndex = allPacientes.FindIndex(p => p.ID == paciente.ID);
        var pacienteSheetraw = await _sheetsDB.LerRangeAsync("Pacientes!A3:O"); // ou outro range total
        var pacienteSheet = pacienteSheetraw.ToList();
        int linhaIndexPaciente = pacienteSheet.FindIndex(r => r.Count > 0 && r[4]?.ToString() == id); // Supondo que a coluna AJ (índice 35) seja o ID

        if (linhaIndexPaciente == -1)throw new Exception("Paciente não encontrado na aba Prontuario.");
        // Passo 2: A linha no Google Sheets começa em 2 (1 para header)
        int linhaPacienteNoSheet = linhaIndexPaciente + 3;

        // if (linhaProntuarioNoSheet == -1)
        //     throw new Exception("Paciente não encontrado na planilha.");

        // // Passo 2: A linha no Google Sheets começa em 2 (1 para header)
        // int linhaNoSheet = linhaProntuarioNoSheet + 2;
        Console.WriteLine($"A linha deletada será {linhaPacienteNoSheet}");

        await _sheetsDB.DeleteLineAsync(linhaPacienteNoSheet, "Pacientes");
    }
    public Paciente InstantiatePaciente(IList<object> row)
    {
        var nascimentoString = row[2]?.ToString();
        DateOnly nascimento = DateOnly.MinValue;
        if (!string.IsNullOrWhiteSpace(nascimentoString))
        {
            DateOnly.TryParse(nascimentoString, out nascimento);
        }
        return new Paciente
                {
                    CPF = row[0].ToString() ?? null,
                    Nome = row[1].ToString() ?? null,
                    Nascimento = nascimento,
                    Plano = row[3].ToString() ?? null,
                    ID = row[4].ToString(),
                    Carteira = row[5]?.ToString() ?? null,
                    Email = row[6]?.ToString() ?? null,
                    Telefone = row[7]?.ToString() ?? null,
                    Endereco = new Endereco
                    {
                        Logradouro = row[8].ToString(),
                        Numero = row[9]?.ToString(),
                        Bairro = row[10]?.ToString(),
                        Cidade = row[11]?.ToString(),
                        UF = row[12]?.ToString(),
                        CEP = row[13]?.ToString()
                    },
                    RG = row.Count > 14 ? row[14]?.ToString() : null
                };
    }
    public ValueRange  CreatePacienteToSheet( Paciente paciente)
    {
        return new ValueRange
        {
            Values = new List<IList<object>> {
                new List<object> {
                    paciente.CPF ?? "0",
                    paciente.Nome ?? "0",
                    paciente.Nascimento.ToString("dd/MM/yyyy"),
                    paciente.Plano ?? "0",
                    paciente.ID ?? "0",
                    paciente.Carteira ?? "0",
                    paciente.Email ?? "0",
                    paciente.Telefone ?? "0",
                    paciente.Endereco?.Logradouro ?? "0",
                    paciente.Endereco?.Numero ?? "0",
                    paciente.Endereco?.Bairro ?? "0",
                    paciente.Endereco?.Cidade ?? "0",
                    paciente.Endereco?.UF ?? "0",
                    paciente.Endereco?.CEP ?? "0",
                    paciente.RG ?? "0"
                }
            }
        };
    }
    public enum PacientesFilter
    {
        [Display(Name = "CPF")]
        Cpf = 0,
        [Display(Name = "Nome")]
        Nome = 1,
        [Display(Name = "Data de nascimeto")]
        DataNasimento = 2,
        [Display(Name = "Plano de saúde")]
        PlanoSaude = 3,
        [Display(Name = "ID")]
        Id = 4,
        [Display(Name = "Carteira")]
        Carteira = 5,
        [Display(Name = "email")]
        Email = 6,
        [Display(Name = "celular")]
        Celular = 8,
        [Display(Name = "Logradouro")]
        Logradouro = 9,
        [Display(Name = "Número")]
        Numero = 10,
        [Display(Name = "Bairro")]
        Bairro = 11,
        [Display(Name = "Cidade")]
        Cidade = 12,
        [Display(Name = "Estado")]
        Estado = 13,
        [Display(Name = "CEP")]
        Cep = 14,
        [Display(Name = "RG")]
        Rg = 15,
    }
    

}


