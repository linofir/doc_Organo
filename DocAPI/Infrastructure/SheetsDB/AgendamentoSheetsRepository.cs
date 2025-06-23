using System.ComponentModel.DataAnnotations;
using System.Reflection;
using DocAPI.Core.Models;
using DocAPI.Core.Repositories;
using DocAPI.Services;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using NPOI.HSSF.Record;
using Org.BouncyCastle.Crypto.Digests;
using static DocAPI.Core.Models.Agendamento;

namespace DocAPI.Infrastructure.Sheets;

public class AgendamentoSheetsRepository : IAgendamentoRepository
{
    private readonly GoogleSheetsDB _sheetsDB;
    public AgendamentoSheetsRepository(GoogleSheetsDB sheets)
    {
        _sheetsDB = sheets;
    }
    public async Task<IEnumerable<Agendamento>> GetAllAsync(int skip = 0, int take = 10)
    {
        var agendamentos = await GetAgendamentosAsync();
        return agendamentos.Skip(skip).Take(take);
        // throw new NotImplementedException();
    }
    public async Task<Agendamento?> GetByIdAsync(string id)
    {
        Console.WriteLine($"Interface de Id do agendamento...");
        var agendamentos = await GetAgendamentosAsync();
        var agendamento = agendamentos.FirstOrDefault(p =>
            !string.IsNullOrWhiteSpace(p.ID) &&
            !string.IsNullOrWhiteSpace(id) &&
            p.ID.Trim().Equals(id.Trim(), StringComparison.OrdinalIgnoreCase));

        if (agendamento == null)
        {
            Console.WriteLine($"❌ Agendamento com ID '{id}' não encontrado.");
        }
        return agendamento;
    }
    public async Task<List<Agendamento>> GetByNameAsync(string name)
    {
        return await GetAgendamentoByFilterAsync( name, AgendamentosFilter.Nome );
    }
    public async Task<List<Agendamento>> GetByPacienteIdAsync(string pacienteId)
    {
        Console.WriteLine($"Interface de pacienteID...");
        return await GetAgendamentoByFilterAsync( pacienteId, AgendamentosFilter.PacienteId);
    }
    public async Task CreateAsync(Agendamento novoAgendamento)
    {
        await AddAgendamentoAsync(novoAgendamento);
        Console.WriteLine($"Armazenamento no fim...");
    }
    public async Task UpdateAsync(Agendamento agendamento, string id)
    {
        await UpdateAgendamentoAsync(agendamento, id);
        // throw new NotImplementedException();
    }
    public async Task DeleteAsync(string id)
    {
        await DeleteAgendamentoAsync(id);
        // throw new NotImplementedException();
    }
/// metodos
    public async Task<List<Agendamento>> GetAgendamentosAsync()
    {
        var values = await _sheetsDB.LerRangeAsync("Agendamentos!A3:N"); // de A até a coluna ID
        var allAgendamentos = new List<Agendamento>();
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
                var horarioColuna = row[3]?.ToString();
                TimeOnly horario = TimeOnly.MinValue;
                if (!string.IsNullOrWhiteSpace(horarioColuna))
                {
                    TimeOnly.TryParse(horarioColuna, out horario);
                }
                var agendamento = InstantiateAgendamento(row);
                allAgendamentos.Add(agendamento);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar linha {i + 3}: {ex.Message}");
            }
        }
        return allAgendamentos;
    }
    public async Task<List<Agendamento>> GetAgendamentoByFilterAsync(string conditionOfRow, AgendamentosFilter rowIndex)
    {
        Console.WriteLine($"Filtrando agendamento da coluna(i): {rowIndex } para {conditionOfRow}" );
        var values = await _sheetsDB.LerRangeAsync("Agendamentos!A3:N"); // de A até a coluna ID
        var agendamentosOfcondition = new List<Agendamento>{};
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
                    Agendamento agendamento = InstantiateAgendamento(row);
                    agendamentosOfcondition.Add(agendamento);
                    Console.WriteLine($"O Agendamento da paciente {agendamento.Nome}, foi encontrado");
                }
                
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Erro ao processar linha {i + 3}: {ex.Message}");
            }

        }
        return agendamentosOfcondition;
        
    }
    public async Task AddAgendamentoAsync(Agendamento agendamento)
    {
        Console.WriteLine($"Comunicando com DB..." );
        var agendamentoSheets = await _sheetsDB.LerRangeAsync("Agendamentos!A3:N");
        int novaLinhaIndex = agendamentoSheets.Count(r => r.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString()))) + 3;
        //Como é melhor armazenar o status, é preciso validar e preparar os dados antes de armazena-los
        if (string.IsNullOrEmpty(agendamento.ID))
        {
            agendamento.ID = Guid.NewGuid().ToString();
        };


        ValueRange body = CreateAgendamentoToSheets(agendamento);
        
        // 3. Escrever os dados na próxima linha disponível
        string rangeDestino = $"Agendamentos!A{novaLinhaIndex}:N{novaLinhaIndex}";
        Console.WriteLine($"O novo Agendamento será acrescentado na { rangeDestino}");
        await _sheetsDB.WriteRangeAsync(rangeDestino, body.Values);
    }
    public async Task UpdateAgendamentoAsync(Agendamento agendamento, string id)
    {
        // Passo 1: Buscar a linha do agendamento (por ID), repensar como identifiar a linha na planilha
        var allAgendamentos = await _sheetsDB.LerRangeAsync("Agendamentos!A3:AJ");
        var sheetFilter = (int)AgendamentosFilter.Id;
        int linhaIndex = allAgendamentos.ToList().FindIndex(r => {
        // Garante que a linha tem colunas suficientes e o valor da célula não é nulo/vazio
        if (r.Count > sheetFilter && !string.IsNullOrWhiteSpace(r[sheetFilter]?.ToString()))
        {
            return id.Trim().Equals(r[sheetFilter].ToString()!.Trim(), StringComparison.OrdinalIgnoreCase);
        }
        return false;
        });

        if (linhaIndex == -1)
            throw new Exception("Agendamento não encontrado na planilha.");

        // Passo 2: A linha no Google Sheets começa em 3 (1 para header)
        int linhaNoSheet = linhaIndex + 3;

        // Passo 3: Criar os dados atualizados
        var valoresAtualizados = CreateAgendamentoToSheets(agendamento);

        // 5. Montar o range da linha específica (A até N)
        string range = $"Agendamentos!A{linhaNoSheet}:N{linhaNoSheet}";

        Console.WriteLine($"Atualizando agendamento com ID '{agendamento.ID}' na linha {linhaNoSheet}");

        // 6. Escrever na planilha
        await _sheetsDB.WriteRangeAsync(range, valoresAtualizados.Values);
    }
    public async Task DeleteAgendamentoAsync(string id)
    {
        var allAgendamentos = await _sheetsDB.LerRangeAsync("Agendamentos!A3:AJ");
        int linhaIndex = allAgendamentos.ToList().FindIndex(r => {
        // Garante que a linha tem colunas suficientes e o valor da célula não é nulo/vazio
        if (r.Count > (int)AgendamentosFilter.Id && !string.IsNullOrWhiteSpace(r[(int)AgendamentosFilter.Id]?.ToString()))
        {
            return id.Trim().Equals(r[(int)AgendamentosFilter.Id].ToString()!.Trim(), StringComparison.OrdinalIgnoreCase);
        }
        return false;
        });

        if (linhaIndex == -1)
            throw new Exception("Agendamento não encontrado na planilha.");

        // Passo 2: A linha no Google Sheets começa em 3 (1 para header)
        int linhaNoSheet = linhaIndex + 3;
        await _sheetsDB.DeleteLineAsync(linhaNoSheet, "Agendamentos");
        Console.WriteLine($"A linha deletada será {linhaNoSheet}");
    }
    public ValueRange CreateAgendamentoToSheets( Agendamento agendamento)
    {
        var displayStatus = agendamento.Status
            .GetType()
            .GetMember(agendamento.Status.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            ?.Name ?? agendamento.Status.ToString();
        return new ValueRange
        {
            Values = new List<IList<object>> {
                new List<object> {
                    agendamento.Nome,
                    agendamento.Aviso,
                    agendamento.Data.ToString("yyy-MM-dd") ?? null,
                    agendamento.Horario.ToString("HH-mm"),
                    agendamento.Procedimento,
                    agendamento.Local,
                    agendamento.Sala,
                    agendamento.Status,
                    agendamento.SenhaAgendamento.Codigo,
                    agendamento.SenhaAgendamento.DataPedido.ToString("yyy-MM-dd") ?? null,
                    agendamento.SenhaAgendamento.DataLibetracao?.ToString("yyy-MM-dd") ?? null,
                    agendamento.SenhaAgendamento.Validade?.ToString("yyy-MM-dd") ?? null,
                    agendamento.PacienteID,
                    agendamento.ID,
                }
            }
        };
    }
    public Agendamento InstantiateAgendamento( IList<object> row)
    {
        var horarioColuna = row[3]?.ToString();
        TimeOnly horario = TimeOnly.MinValue;
        if (!string.IsNullOrWhiteSpace(horarioColuna))
        {
            TimeOnly.TryParse(horarioColuna, out horario);
        }
        var agendamento = new Agendamento
        {
            ID = row[13]?.ToString() ?? "",
            PacienteID = row[12].ToString() ?? "",
            Nome = row[0]?.ToString() ?? "",
            Aviso = row[1]?.ToString() ?? "",
            Data = ParseDateOnly(row[2]?.ToString() ?? ""),
            Horario = horario,
            Procedimento = row[4]?.ToString() ?? "",
            Local = row[5]?.ToString() ?? "",
            Sala = row[6]?.ToString() ?? "",
            Status = ParseStatusAgendamento(row[7]?.ToString()),
            SenhaAgendamento = new Senha(){
                Codigo = row[8]?.ToString() ?? "",
                DataPedido = ParseDateOnly(row[9]?.ToString() ?? ""),
                DataLibetracao = ParseDateOnly(row[10]?.ToString() ?? ""),
                Validade = ParseDateOnly(row[11]?.ToString() ?? ""),
                }
        };
        return agendamento;
    }
    public static StatusAgendamento ParseStatusAgendamento(string? valor)
    {
        return valor?.Trim() switch
        {
            "Sem senha" => StatusAgendamento.SemSenha,
            "Senha pendente" => StatusAgendamento.SenhaPendente,
            "Senha Aprovada" => StatusAgendamento.SenhaAprovada,
            "Agendamento Efetuado" => StatusAgendamento.AgendamentoEfetuado,
            "Agendamento Remarcado" => StatusAgendamento.AgendamentoRemarcado,
            "Concluida" => StatusAgendamento.ProcedimentoConcluido,
            "Expirada" or "Cancelada" => StatusAgendamento.Cancelada,

            _ => throw new ArgumentException($"Valor inválido para StatusVacinaHPV: '{valor}'")
        };
    }
    public DateOnly ParseDateOnly(string datecolumn)
    {
        var datacoluna = datecolumn;
        DateOnly data = DateOnly.MinValue;
        if (!string.IsNullOrWhiteSpace(datacoluna))
        {
            DateOnly.TryParse(datacoluna, out data);
        }
        return data;
    }
    public enum AgendamentosFilter
    {
        [Display(Name = "Nome")]
        Nome = 0,
        [Display(Name = "Aviso")]
        Aviso = 1,
        [Display(Name = "Data")]
        Data = 2,
        [Display(Name = "Horário")]
        Horario = 3,
        [Display(Name = "Procedimento")]
        Procedimento = 4,
        [Display(Name = "Local")]
        Local = 5,
        [Display(Name = "Sala")]
        Sala = 6,
        [Display(Name = "Senha")]
        Senha = 8,
        [Display(Name = "DataPedido")]
        DataPedido = 9,
        [Display(Name = "DataLiberação")]
        DataLiberacao = 10,
        [Display(Name = "Validade")]
        Validade = 11,
        [Display(Name = "PacienteId")]
        PacienteId = 12,
        [Display(Name = "ID")]
        Id = 13,

    }
    public Senha CheckSenhaAgendmaneto(StatusAgendamento status)
    {
        if( status == StatusAgendamento.SemSenha) 
        {
            return new Senha(){};
        }else
        {
            Console.WriteLine("checagem de senha ainda não foi implementada, retornará nula");
            return new Senha(){};
        }


    }
}
