using System.Text.Json;
using DocAPI.Core.Models;
using DocAPI.Core.Repositories;
using DocAPI.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Extensions.WebEncoders.Testing;

namespace DocAPI.Infrastructure.Sheets;

public class ProntuarioSheetsRepository : IProntuarioRepository
{
    private readonly GoogleSheetsDB _sheetsDB;
    private readonly IPacienteRepository _iPacienteRepository;
    public ProntuarioSheetsRepository(GoogleSheetsDB sheets, IPacienteRepository iPacienteRepository)
    {
        _sheetsDB = sheets;
        _iPacienteRepository = iPacienteRepository;
    }
    public async Task<IEnumerable<Prontuario>> GetAllAsync(int skip = 0, int take = 10)
    {
        var prontuarios = await GetProntuariosAsync();
        return prontuarios.Skip(skip).Take(take);
    }
    public async Task<Prontuario?> GetByIdAsync(string id)
    {
        var prontuarios = await GetProntuariosAsync();
        return prontuarios.FirstOrDefault(p => p.ID == id);
        //throw new NotImplementedException(); 
    }

    public  Task CreateAsync(Prontuario prontuario)
    {
        // await AddPacienteAsync( paciente);
        throw new NotImplementedException();
    }

    public  Task UpdateAsync(Prontuario prontuario, string id)
    {
        // await UpdatePacienteAsync(paciente, id);
        throw new NotImplementedException();
    }

    public  Task DeleteAsync(string id)
    {
        // await DeletePacienteAsync(id);
        throw new NotImplementedException();
    }
    
    public async Task<List<Prontuario>> GetProntuariosAsync()
    {
        var prontuarioSheet = await _sheetsDB.LerRangeAsync("Prontuario!A2:AJ");
        var pedidosExameSheet = await _sheetsDB.LerRangeAsync("PedidosExame!A2:D");
        var pedidosCirurgiaSheet = await _sheetsDB.LerRangeAsync("PedidosCirurgia!A2:P");

        var prontuarios = new List<Prontuario>();
        int linhaAtual = 1;

        foreach (var row in prontuarioSheet.Skip(1)) // Ignora o cabeçalho
        {
            if (row.All(cell => string.IsNullOrWhiteSpace(cell?.ToString()))) 
            {
                continue;
            }
            if (row.Count != 36)
            {
                linhaAtual++;
                Console.WriteLine($"Linha {linhaAtual} com a quantidade de colunas igual á {row.Count} ");
                //inválida: " + string.Join(",", row ?? new List<object>())
                continue;
            } 
            var id = row[35]?.ToString(); // Coluna AJ (ID do Prontuário)
            var pacienteId = row[34]?.ToString().Trim(); // Coluna AI (ID do Paciente)
            // Console.WriteLine(pacienteId);


            var paciente = new Paciente();
            paciente = await _iPacienteRepository.GetByIdAsync(pacienteId); 
            if (paciente == null)
            {
                Console.WriteLine($"Paciente com ID {pacienteId} não encontrado.");
                continue; // ou retorne erro apropriado, dependendo do contexto
            }
            // Console.WriteLine($"Paciente carregado: {JsonSerializer.Serialize(paciente)}");

            var prontuario = new Prontuario(paciente)
            {
                ID = id,
                AGO = new AGO
                {
                    DUM = row[8]?.ToString(),
                    Paridade = row[9]?.ToString(),
                    DesejoGestacao = row[10]?.ToString(),
                    Intercorrencias = row[11]?.ToString(),
                    Amamentacao = row[12]?.ToString(),
                    VidaSexual = row[13]?.ToString(),
                    Relacionamento = row[14]?.ToString(),
                    Parceiros = row[15]?.ToString(),
                    Coitarca = row[16]?.ToString(),
                    IST = row[17]?.ToString(),
                    VacinaHPV = ParseVacinaHPV(row[18]?.ToString()),
                    CCO = row[19]?.ToString(),
                    MAC_TRH = row[20]?.ToString()
                },
                Antecedentes = new Antecedentes
                {
                    Comorbidades = row[21]?.ToString(),
                    Medicacao = row[22]?.ToString(),
                    Neoplasias = row[23]?.ToString(),
                    Cirurgias = row[24]?.ToString(),
                    Alergias = row[25]?.ToString(),
                    Vicios = row[26]?.ToString(),
                    HabitoIntestinal = row[27]?.ToString(),
                    Vacinas = row[28]?.ToString()
                },
                AntecedentesFamiliares = new AntecedentesFamiliares
                {
                    Neoplasias = row[29]?.ToString(),
                    Comorbidades = row[30]?.ToString()
                },
                InformacoesExtras = row[31]?.ToString(),
                CD = ParseCd(row[33]?.ToString().Split(',').ToList())
            };
            prontuario.DescricaoBasica.Profissao = row[3]?.ToString();
            prontuario.DescricaoBasica.Religiao = row[4]?.ToString();
            prontuario.DescricaoBasica.QD = row[5]?.ToString();

            foreach (var rowExame in pedidosExameSheet.Skip(1)) // Ignora o cabeçalho
            {
                if (rowExame.All(cell => string.IsNullOrWhiteSpace(cell?.ToString()))) continue;
                if (rowExame.Count != 4) 
                {
                    Console.WriteLine("Linha inválida exame: " + string.Join(",", rowExame ?? new List<object>()));
                    continue;
                }
                if (rowExame[3]!.ToString() == prontuario.ID )
                {

                    List<Exame> exames = new List<Exame>();
                    rowExame[2]!.ToString().Split(',').ToList().ForEach(e => exames.Add(new Exame
                    {
                        Codigo = "",
                        Nome = e
                    }));
                    prontuario.Exames = exames;
                    
                }
                
            };

            foreach (var rowCirurgia in pedidosCirurgiaSheet.Skip(1)) // Ignora o cabeçalho
            {
                if (rowCirurgia.All(cell => string.IsNullOrWhiteSpace(cell?.ToString()))) continue;
                if (rowCirurgia.Count != 16) 
                {
                    Console.WriteLine("Linha inválida cirurgia: " + string.Join(",", rowCirurgia ?? new List<object>()));
                    continue;
                }
                if (rowCirurgia[15]!.ToString() == prontuario.ID )
                {
                    prontuario.SolicitacaoInternacao = new Internacao
                {
                    Procedimentos = rowCirurgia[2]!.ToString().Split(',').ToList(),
                    IndicaçãoClinica = rowCirurgia[3]!.ToString(),
                    Observacao = rowCirurgia[4]!.ToString(),
                    CID = rowCirurgia[5]!.ToString(),
                    Data = DateTime.Parse(rowCirurgia[1]!.ToString()),
                    Carater = rowCirurgia[10]!.ToString(),
                    Regime = rowCirurgia[9]!.ToString(),
                    UsaOPME = rowCirurgia[11]!.ToString().ToLower().Contains("sim"),
                    Local = rowCirurgia[12]!.ToString(),
                    Guia = long.TryParse(rowCirurgia[13]!.ToString(), out var guia) ? guia : 0
                };
                }
            };


            prontuarios.Add(prontuario);
            }

        return prontuarios;
    }
    // public static bool TryParseVacinaHPV(string? valor, out StatusVacinaHPV status)
    // {
    //     switch (valor?.Trim())
    //     {
    //         case "Sim, 1 dose":
    //             status = StatusVacinaHPV.UmaDose;
    //             return true;
    //         case "Sim, 2 doses":
    //             status = StatusVacinaHPV.DuasDoses;
    //             return true;
    //         case "Sim, 3 doses":
    //             status = StatusVacinaHPV.TresDoses;
    //             return true;
    //         case "Sem vacina":
    //             status = StatusVacinaHPV.SemVacina;
    //             return true;
    //         case "Sem info":
    //         case "Sem informação":
    //             status = StatusVacinaHPV.SemInfo;
    //             return true;
    //         default:
    //             status = StatusVacinaHPV.SemInfo;
    //             return false;
    //     }
    // }
    public static List<AcoesCD> ParseCd(List<string>? acoesList)
    {
        if (acoesList == null) return new();

        return acoesList
            .Select(a =>
            {
                var valor = a.Trim().ToLowerInvariant();
                return valor switch
                {
                    "pedido de internação" => AcoesCD.PedidoInternacao,
                    "pedido de exame" => AcoesCD.PedidoExame,
                    "indicação de encaminhamentos" => AcoesCD.IndicacaoEncaminhamentos,
                    "informativos de instrumentadora" => AcoesCD.InformativosInstrumentadora,
                    "termo cirúrgico" => AcoesCD.TermoCirurgico,
                    "pasta informativa" => AcoesCD.PastaInformativa,
                    "sem info" => AcoesCD.SemInformacao,
                    _ => throw new ArgumentException($"Valor inválido para AcoesCD: '{a}'")
                };
            })
            .ToList();
    }   
    public static StatusVacinaHPV ParseVacinaHPV(string? valor)
    {
        return valor?.Trim() switch
        {
            "Sim, 1 dose" => StatusVacinaHPV.UmaDose,
            "Sim, 2 doses" => StatusVacinaHPV.DuasDoses,
            "Sim, 3 doses" => StatusVacinaHPV.TresDoses,
            "Sem Vacina" => StatusVacinaHPV.SemVacina,
            "Sem info" or "Sem informação" => StatusVacinaHPV.SemInfo,
            _ => throw new ArgumentException($"Valor inválido para StatusVacinaHPV: '{valor}'")
        };
    }

    // public async Task<List<Prontuario>> GetProntuariosAsync()
    // {
    //     var prontuarioSheet = await _sheetsDB.LerRangeAsync("Prontuario!A2:AJ");
    //     var pedidosExameSheet = await _sheetsDB.LerRangeAsync("PedidosExame!A2:D");
    //     var pedidosCirurgiaSheet = await _sheetsDB.LerRangeAsync("PedidosCirurgia!A2:P");

    //     var prontuarios = new List<Prontuario>();
    //     int linhaAtual = 1;

    //     foreach (var row in prontuarioSheet.Skip(1)) // Ignora o cabeçalho
    //     {
    //         if (row.All(cell => string.IsNullOrWhiteSpace(cell?.ToString()))) 
    //         {
    //             continue;
    //         }
    //         if (row.Count != 36)
    //         {
    //             linhaAtual++;
    //             Console.WriteLine($"Linha {linhaAtual} com a quantidade de colunas igual á {row.Count} ");
    //             //inválida: " + string.Join(",", row ?? new List<object>())
    //             continue;
    //         } 
    //         var id = row[35]?.ToString(); // Coluna AJ (ID do Prontuário)
    //         var pacienteId = row[34]?.ToString().Trim(); // Coluna AI (ID do Paciente)
    //         if (string.IsNullOrWhiteSpace(pacienteId))
    //         {
    //             Console.WriteLine($"❌ ID do paciente está vazio ou nulo na linha {linhaAtual}.");
    //             linhaAtual++;
    //             continue;
    //         }

    //         Prontuario prontuario = null;
    //         try
    //         {
    //             var paciente = await _iPacienteRepository.GetByIdAsync(pacienteId);

    //             if (paciente == null)
    //             {
    //                 Console.WriteLine($"❌ Paciente com ID {pacienteId} não encontrado na linha {linhaAtual}.");
    //                 linhaAtual++;
    //                 continue;
    //             }

    //             Console.WriteLine($"✅ Paciente carregado: {JsonSerializer.Serialize(paciente)}");

    //             prontuario = new Prontuario(paciente)
    //             {
    //                 ID = id,
    //                 AGO = new AGO
    //                 {
    //                     DUM = row[8]?.ToString(),
    //                     Paridade = row[9]?.ToString(),
    //                     DesejoGestacao = row[10]?.ToString(),
    //                     Intercorrencias = row[11]?.ToString(),
    //                     Amamentacao = row[12]?.ToString(),
    //                     VidaSexual = row[13]?.ToString(),
    //                     Relacionamento = row[14]?.ToString(),
    //                     Parceiros = row[15]?.ToString(),
    //                     Coitarca = row[16]?.ToString(),
    //                     IST = row[17]?.ToString(),
    //                     VacinaHPV = Enum.TryParse<StatusVacinaHPV>(row[18]?.ToString(), out var vacina) ? vacina : StatusVacinaHPV.SemVacina,
    //                     CCO = row[19]?.ToString(),
    //                     MAC_TRH = row[20]?.ToString()
    //                 },
    //                 Antecedentes = new Antecedentes
    //                 {
    //                     Comorbidades = row[21]?.ToString(),
    //                     Medicacao = row[22]?.ToString(),
    //                     Neoplasias = row[23]?.ToString(),
    //                     Cirurgias = row[24]?.ToString(),
    //                     Alergias = row[25]?.ToString(),
    //                     Vicios = row[26]?.ToString(),
    //                     HabitoIntestinal = row[27]?.ToString(),
    //                     Vacinas = row[28]?.ToString()
    //                 },
    //                 AntecedentesFamiliares = new AntecedentesFamiliares
    //                 {
    //                     Neoplasias = row[29]?.ToString(),
    //                     Comorbidades = row[30]?.ToString()
    //                 },
    //                 InformacoesExtras = row[31]?.ToString()
    //             };

    //             prontuario.DescricaoBasica.Profissao = row[3]?.ToString();
    //             prontuario.DescricaoBasica.Religiao = row[4]?.ToString();
    //             prontuario.DescricaoBasica.QD = row[5]?.ToString();
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.WriteLine($"❌ Erro ao buscar paciente ou montar prontuário (linha {linhaAtual} - ID {pacienteId}): {ex.Message}");
    //             linhaAtual++;
    //             continue;
    //         }

    //         prontuarios.Add(prontuario);
    //         }

    //     return prontuarios;
    // }


    // public async Task AddProntuatioAsync(Prontuario paciente)
    // {
    //     // 1. Ler as linhas existentes
    //     var valores = await _sheetsDB.LerRangeAsync("Pacientes!A3:O");
    //     int novaLinhaIndex = valores.Count + 3; // +3 porque a planilha começa na linha 3
        // 2. Preparar os valores a serem inseridos
        // ValueRange body = new()
        // {
        //     Values = new List<IList<object>> {
        //         new List<object> {
        //             paciente.CPF,
        //             paciente.Nome,
        //             paciente.Nascimento.ToString("dd/MM/yyyy"),
        //             paciente.Plano,
        //             paciente.ID,
        //             paciente.Carteira,
        //             paciente.Email,
        //             paciente.Telefone,
        //             paciente.Endereco?.Logradouro,
        //             paciente.Endereco?.Numero,
        //             paciente.Endereco?.Bairro,
        //             paciente.Endereco?.Cidade,
        //             paciente.Endereco?.UF,
        //             paciente.Endereco?.CEP,
        //             paciente.RG
        //         }
        //     }
        // };


    //     // 3. Escrever os dados na próxima linha disponível
    //     string rangeDestino = $"Pacientes!A{novaLinhaIndex}:O{novaLinhaIndex}";
    //     Console.WriteLine($"A nova Paciente será acrescentada na { rangeDestino}");
    //     await _sheetsDB.WriteRangeAsync(rangeDestino, body.Values);
    // }
    // public async Task UpdatePacienteAsync(Paciente paciente, string id)
    // {
    //     // Passo 1: Buscar a linha do paciente (por ID)
    //     var allPacientes = await GetPacientesAsync();
    //     int linhaIndex = allPacientes.FindIndex(p => p.ID == id);
    //     //int linhaIndex = allPacientes.FindIndex(p => p.ID == paciente.ID);

    //     if (linhaIndex == -1)
    //         throw new Exception("Paciente não encontrado na planilha.");

    //     // Passo 2: A linha no Google Sheets começa em 2 (1 para header)
    //     int linhaNoSheet = linhaIndex + 3;

    //     // Passo 3: Criar os dados atualizados
    //     var valoresAtualizados = new List<IList<object?>>
    //     {
    //         new List<object?>
    //         {
    //             paciente.CPF,
    //             paciente.Nome,
    //             paciente.Nascimento.ToString("dd/MM/yyyy"),
    //             paciente.Plano,
    //             paciente.ID,
    //             paciente.Carteira,
    //             paciente.Email,
    //             paciente.Telefone,
    //             paciente.Endereco?.Logradouro,
    //             paciente.Endereco?.Numero,
    //             paciente.Endereco?.Bairro,
    //             paciente.Endereco?.Cidade,
    //             paciente.Endereco?.UF,
    //             paciente.Endereco?.CEP,
    //             paciente.RG
    //         }
    //     };

    //     // 5. Montar o range da linha específica (A até O)
    //     string range = $"Pacientes!A{linhaNoSheet}:O{linhaNoSheet}";

    //     Console.WriteLine($"Atualizando paciente com ID '{paciente.ID}' na linha {linhaNoSheet}");

    //     // 6. Escrever na planilha
    //     await _sheetsDB.WriteRangeAsync(range, valoresAtualizados);
    // }
    // public async Task DeletePacienteAsync(string id)
    // {

    //     // Passo 1: Buscar a linha do paciente (por ID)
    //     var allPacientes = await GetPacientesAsync();
    //     int linhaIndex = allPacientes.FindIndex(p => p.ID == id);
    //     //int linhaIndex = allPacientes.FindIndex(p => p.ID == paciente.ID);

    //     if (linhaIndex == -1)
    //         throw new Exception("Paciente não encontrado na planilha.");

    //     // Passo 2: A linha no Google Sheets começa em 2 (1 para header)
    //     int linhaNoSheet = linhaIndex + 2;
    //     Console.WriteLine($"A linha deletada será {linhaNoSheet}");

    //     await _sheetsDB.DeleteLineAsync(linhaNoSheet, "Pacientes");
    // }
    


}


