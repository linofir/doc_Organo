using System.ComponentModel.DataAnnotations;
using System.Reflection;
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

    public async Task CreateAsync(Prontuario prontuario)
    {
        await AddProntuarioAsync( prontuario);
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
                    Data = DateTime.Parse(rowCirurgia[1]!.ToString()),
                    Procedimentos = rowCirurgia[2]!.ToString().Split(',').ToList(),
                    IndicacaoClinica = rowCirurgia[3]!.ToString(),
                    Observacao = rowCirurgia[4]!.ToString(),
                    CID = rowCirurgia[5]!.ToString(),
                    TempoDoenca = rowCirurgia[6]!.ToString(),
                    Diarias = rowCirurgia[7]!.ToString(),
                    Tipo = rowCirurgia[8]!.ToString(),
                    Regime = rowCirurgia[9]!.ToString(),
                    Carater = rowCirurgia[10]!.ToString(),
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
    public async Task AddProntuarioAsync(Prontuario prontuario)
    {
        // 1. Ler as linhas existentes
        var prontuarioSheet = await _sheetsDB.LerRangeAsync("Prontuario!A2:AJ");
        var pedidosExameSheet = await _sheetsDB.LerRangeAsync("PedidosExame!A2:D");
        var pedidosCirurgiaSheet = await _sheetsDB.LerRangeAsync("PedidosCirurgia!A2:P");
        int novaLinhaProntuarioIndex = prontuarioSheet.Count(r => r.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString()))) + 2;
        // int novaLinhaProntuarioIndex = prontuarioSheet.Count + 1; 
        int novaLinhaExameIndex = pedidosExameSheet.Count(r => r.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString()))) + 2;
        // int novaLinhaExameIndex = pedidosExameSheet.Count + 1; 
        int novaLinhaCirurgiaIndex = pedidosCirurgiaSheet.Count(r => r.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString()))) + 2;
        // int novaLinhaCrirurgiaIndex = pedidosCirurgiaSheet.Count + 1; 
        //2. Preparar os valores a serem inseridos
        var descricao = prontuario.DescricaoBasica;
        var ago = prontuario.AGO;
        var ap = prontuario.Antecedentes;
        var af = prontuario.AntecedentesFamiliares;
        var paciente = await _iPacienteRepository.GetByIdAsync(prontuario.DescricaoBasica.PacienteId);

        if (paciente == null)
        {
            throw new Exception("Paciente não encontrado.");
        }
        if (string.IsNullOrEmpty(prontuario.ID))
        {
            prontuario.ID = Guid.NewGuid().ToString();
        }
        Console.WriteLine(prontuario.ID);
        var displayVacina = ago.VacinaHPV
            .GetType()
            .GetMember(ago.VacinaHPV.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            ?.Name ?? ago.VacinaHPV.ToString();

        var acoes = prontuario.CD != null
            ? string.Join(", ", prontuario.CD.Select(cd =>
                cd.GetType()
                .GetMember(cd.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()?.Name ?? cd.ToString()))
            : "";

    var dataHoje = DateTime.Now.ToString("dd/MM/yyyy");
        ValueRange bodyProntuario = new()
        {
            Values = new List<IList<object>> {
                new List<object> {
                    descricao.NomePaciente,
                    descricao.Cpf,
                    descricao.Idade,
                    descricao.Profissao,
                    descricao.Religiao,
                    descricao.QD,
                    descricao.AtividadeFisica,
                    ago.Menarca,
                    ago.DUM,
                    ago.Paridade,
                    ago.DesejoGestacao,
                    ago.Intercorrencias,
                    ago.Amamentacao,
                    ago.VidaSexual,
                    ago.Relacionamento,
                    ago.Parceiros,
                    ago.Coitarca,
                    ago.IST,
                    displayVacina,
                    ago.CCO,
                    ago.MAC_TRH,
                    ap.Comorbidades,
                    ap.Medicacao,
                    ap.Neoplasias,
                    ap.Cirurgias,
                    ap.Alergias,
                    ap.Vicios,
                    ap.HabitoIntestinal,
                    ap.Vacinas,
                    af.Neoplasias,
                    af.Comorbidades,
                    prontuario.InformacoesExtras,
                    dataHoje,
                    acoes,
                    descricao.PacienteId,
                    prontuario.ID
                }
            }
        };
        string examesFormatados = string.Join("; ", prontuario.Exames.Select(exame => $"{exame.Codigo} - {exame.Nome}"));
        ValueRange bodyExames = new()
        {
            Values = new List<IList<object>>
            {
                new List<object>
                {
                    descricao.NomePaciente,
                    prontuario.DataRequisicao.ToString("dd/MM/yyyy"),
                    examesFormatados,
                    prontuario.ID
                }
            }
        };
        string procedimentosFormatados = string.Join("; ", prontuario.SolicitacaoInternacao.Procedimentos.Select(proc => $"{proc}"));
        ValueRange bodyCirurgias = new()
        {
            // Values = prontuario.SolicitacaoInternacao?.Procedimentos?.Select(...) ?? new List<IList<object>>()
            Values = new List<IList<object>>
            {
                new List<object>
                {
                    descricao.NomePaciente,
                    prontuario.DataRequisicao.ToString("dd/MM/yyyy"),
                    procedimentosFormatados,
                    prontuario.SolicitacaoInternacao.IndicacaoClinica,
                    prontuario.SolicitacaoInternacao.Observacao,
                    prontuario.SolicitacaoInternacao.CID,
                    prontuario.SolicitacaoInternacao.TempoDoenca, // Tempo da doença - campo não mapeado
                    prontuario.SolicitacaoInternacao.Diarias, // Diárias - campo não mapeado
                    prontuario.SolicitacaoInternacao.Tipo,
                    prontuario.SolicitacaoInternacao.Regime,
                    prontuario.SolicitacaoInternacao.Carater,
                    prontuario.SolicitacaoInternacao.UsaOPME ? "Sim" : "Não",
                    prontuario.SolicitacaoInternacao.Local,
                    prontuario.SolicitacaoInternacao.Guia, // Solicitação
                    prontuario.SolicitacaoInternacao.Guia, // Autorização
                    prontuario.ID
                }
            }
        };
        //3. Escrever os dados na próxima linha disponível
        string rangeDestinoProntuario = $"Prontuario!A{novaLinhaProntuarioIndex}:AJ{novaLinhaProntuarioIndex}";
        Console.WriteLine($"O novo prontuário será acrescentada na { rangeDestinoProntuario }");
        await _sheetsDB.WriteRangeAsync(rangeDestinoProntuario, bodyProntuario.Values);

        string rangeDestinoExames = $"PedidosExame!A{novaLinhaExameIndex}:D{novaLinhaExameIndex}";
        Console.WriteLine($"O novo prontuário será acrescentada na { rangeDestinoExames }");
        await _sheetsDB.WriteRangeAsync(rangeDestinoExames, bodyExames.Values);

        string rangeDestinoCirurgias = $"PedidosCirurgia!A{novaLinhaCirurgiaIndex}:P{novaLinhaCirurgiaIndex}";
        Console.WriteLine($"O novo prontuário será acrescentada na { rangeDestinoCirurgias }");
        await _sheetsDB.WriteRangeAsync(rangeDestinoCirurgias, bodyCirurgias.Values);
    }
    // public async Task UpdateProntuarioAsync(Prontuario prontuario, string id)
    // {
    //     // Passo 1: Buscar a linha do prontExames e pedidosCirurgia (por ID)
    //     var prontuarioSheet = await _sheetsDB.LerRangeAsync("Prontuario!A2:AJ");
    //     var rowProntuario = prontuarioSheet.FirstOrDefault(r => r.Equals(row[35].ToString == id  ));
        
    //     // int linhaIndexProntuario = prontuarioSheet.FindIndex(p => p.ID == id);
        
    //     int linhaIndexExames = await GetAllAsync()
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
}


