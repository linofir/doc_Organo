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
using NPOI.SS.Formula.Functions;

namespace DocAPI.Infrastructure.SheetsDb;

public class ProntuarioSheetsRepository : IProntuarioRepository
{
    private readonly GoogleSheetsDB _sheetsDB;
    private readonly IPacienteRepository _iPacienteRepository;
    private readonly PdfGeneratorService _pdfGeneratorService;
    public ProntuarioSheetsRepository(GoogleSheetsDB sheets, IPacienteRepository iPacienteRepository, PdfGeneratorService pdfGeneratorService)
    {
        _sheetsDB = sheets;
        _iPacienteRepository = iPacienteRepository;
        _pdfGeneratorService = pdfGeneratorService;
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

    public async Task UpdateAsync(Prontuario prontuario, string id)
    {
        await UpdateProntuarioAsync(prontuario, id);
    }

    public async Task DeleteAsync(string id)
    {
        await DeleteProntuarioAsync(id);
        // throw new NotImplementedException();
    }
    public async Task<Prontuario> CreateFromPdfAsync(string pacienteId, string pdfPath)
    {
        
        return await AddProntuarioFromPdfAsync(pacienteId, pdfPath);
    }
    public async Task<List<Prontuario>> GetProntuariosOfPacienteAsync( string pacienteId )
    {
        return await CollectProntuariosOfPacienteAsync(pacienteId);
        // throw new NotImplementedException();
    }
    // public async Task<Stream> CreateReportByIdAsync( string pacienteId )
    // {
    //     var paciente = await  _iPacienteRepository.GetByIdAsync(pacienteId);
    //     if(paciente == null)
    //     {
    //         throw new InvalidOperationException($"Paciente com ID '{pacienteId}' não encontrado.");
    //     }
    //     var prontuariosOfPaciente = await GetProntuariosOfPacienteAsync( paciente);
    //     return _pdfGeneratorService.GeneratePatientReportPdf( paciente, prontuariosOfPaciente);
    // }

    // public async Task<Stream> CreateReportByCpfAsync( string pacienteCpf )
    // {
    //     var paciente = await _iPacienteRepository.GetPacienteByCpfAsync(pacienteCpf);
    //     if(paciente == null) 
    //     {
    //         throw new InvalidOperationException($"Paciente com CPF '{pacienteCpf}' não encontrado.");
    //     }
    //     var prontuariosOfPaciente = await GetProntuariosOfPacienteAsync( paciente[0]);
    //     return _pdfGeneratorService.GeneratePatientReportPdf( paciente[0], prontuariosOfPaciente);
    // }
    //talvez seja melhor fazer uma busca direta
    public async Task<List<Prontuario>> CollectProntuariosOfPacienteAsync(string pacienteId)
    {
        Console.WriteLine($"teste no collect paciente id: {pacienteId}");
        var prontuariosOfPaciente = new List<Prontuario>{};
        var prontuariosList = await GetProntuariosAsync();
        // var pacienteToCheck = await _iPacienteRepository.GetByIdAsync(pacienteId);
        // foreach (var prontuario in prontuariosList)
        // {
        //     var pacienteIdProntuario = prontuario.DescricaoBasica!.PacienteId;
        //     if(pacienteIdProntuario == pacienteId)
        //     {
        //         prontuariosOfPaciente.Add(prontuario);
        //         Console.WriteLine($"Prontuário encontrado da paciente: {prontuario.DescricaoBasica.NomePaciente}");
        //     }
        // }
        return  prontuariosList
            .Where(p => p.DescricaoBasica?.PacienteId == pacienteId)
            .ToList();
        // return prontuariosOfPaciente;
    }
    public async Task<List<Prontuario>> GetProntuariosAsync()
    {
        var prontuarioSheet = await _sheetsDB.LerRangeAsync("Prontuario!A2:AN");
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
            if (row.Count != 40)
            {
                linhaAtual++;
                Console.WriteLine($"Linha {linhaAtual} com a quantidade de colunas igual á {row.Count} ");
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
            var prontuario = new Prontuario()
            {
                ID = id,
                DescricaoBasica = new DescricaoBasica(paciente)
                {
                    Profissao = row[3]?.ToString() ?? "",
                    Religiao = row[4]?.ToString() ?? "",
                    QD = row[5]?.ToString() ?? "",
                    AtividadeFisica = row[6]?.ToString() ?? "",
                    
                },
                AGO = new AGO
                {
                    Menarca = row[7].ToString() ?? "",
                    DUM = row[8].ToString() ?? "",
                    Paridade = row[9].ToString() ?? "",
                    DesejoGestacao = row[10].ToString() ?? "",
                    Intercorrencias = row[11]?.ToString() ?? "",
                    Amamentacao = row[12]?.ToString() ?? "",
                    VidaSexual = row[13]?.ToString() ?? "",
                    Relacionamento = row[14]?.ToString() ?? "",
                    Parceiros = row[15]?.ToString() ?? "",
                    Coitarca = row[16]?.ToString() ?? "",
                    IST = row[17]?.ToString() ?? "",
                    VacinaHPV = ParseVacinaHPV(row[18]?.ToString()),
                    CCO = row[19]?.ToString() ?? "",
                    MAC_TRH = row[20]?.ToString() ?? ""
                },
                Antecedentes = new Antecedentes
                {
                    Comorbidades = row[21]?.ToString() ?? "",
                    Medicacao = row[22]?.ToString() ?? "",
                    Neoplasias = row[23]?.ToString() ?? "",
                    Cirurgias = row[24]?.ToString() ?? "",
                    Alergias = row[25]?.ToString() ?? "",
                    Vicios = row[26]?.ToString() ?? "",
                    HabitoIntestinal = row[27]?.ToString() ?? "",
                    Vacinas = row[28]?.ToString() ?? ""
                },
                AntecedentesFamiliares = new AntecedentesFamiliares
                {
                    Neoplasias = row[29]?.ToString() ?? "",
                    Comorbidades = row[30]?.ToString() ?? ""
                },
                InformacoesExtras = row[31]?.ToString(),
                DataConsulta = ParseDateOnly(row[32]?.ToString()),
                CD = ParseCd(row[33]?.ToString().Split(',').ToList()),
                Tipo = row[36]?.ToString() ?? "",
                PosOperatorio = new PosOp
                {
                    PeriodoSeguimento = row[37]?.ToString(),
                    Conclusao = row[38]?.ToString(),
                    ExameMacro = row[39]?.ToString()
                }
            };
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
                    Data = ParseDateOnly(rowCirurgia[1]!.ToString()) ?? DateOnly.MinValue,
                    Procedimentos = rowCirurgia[2]!.ToString().Split(',').ToList(),
                    IndicacaoClinica = rowCirurgia[3]!.ToString() ?? "",
                    Observacao = rowCirurgia[4]!.ToString() ?? "",
                    CID = rowCirurgia[5]!.ToString() ?? "",
                    TempoDoenca = rowCirurgia[6]!.ToString() ?? "",
                    Diarias = rowCirurgia[7]!.ToString() ?? "",
                    Tipo = rowCirurgia[8]!.ToString() ?? "",
                    Regime = rowCirurgia[9]!.ToString() ?? "",
                    Carater = rowCirurgia[10]!.ToString() ?? "",
                    UsaOPME = rowCirurgia[11]!.ToString().ToLower().Contains("sim"),
                    Local = rowCirurgia[12]!.ToString() ?? "",
                    Guia = rowCirurgia[13]!.ToString() ?? "",
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
        var prontuarioSheet = await _sheetsDB.LerRangeAsync("Prontuario!A2:AN");
        var pedidosExameSheet = await _sheetsDB.LerRangeAsync("PedidosExame!A2:D");
        var pedidosCirurgiaSheet = await _sheetsDB.LerRangeAsync("PedidosCirurgia!A2:P");
        int novaLinhaProntuarioIndex = prontuarioSheet.Count(r => r.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString()))) + 2;
        int novaLinhaExameIndex = pedidosExameSheet.Count(r => r.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString()))) + 2;
        int novaLinhaCirurgiaIndex = pedidosCirurgiaSheet.Count(r => r.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString()))) + 2;
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

        ValueRange bodyProntuario = ConstruirBodyProntuario(prontuario);
        ValueRange bodyExames = ConstruirBodyExames(prontuario);
        ValueRange bodyCirurgias = ConstruirBodyCirurgias(prontuario);
        //3. Escrever os dados na próxima linha disponível
        string rangeDestinoProntuario = $"Prontuario!A{novaLinhaProntuarioIndex}:AN{novaLinhaProntuarioIndex}";
        Console.WriteLine($"O novo prontuário será acrescentada na { rangeDestinoProntuario }");
        await _sheetsDB.WriteRangeAsync(rangeDestinoProntuario, bodyProntuario.Values);

        string rangeDestinoExames = $"PedidosExame!A{novaLinhaExameIndex}:D{novaLinhaExameIndex}";
        Console.WriteLine($"O novo prontuário será acrescentada na { rangeDestinoExames }");
        await _sheetsDB.WriteRangeAsync(rangeDestinoExames, bodyExames.Values);

        string rangeDestinoCirurgias = $"PedidosCirurgia!A{novaLinhaCirurgiaIndex}:P{novaLinhaCirurgiaIndex}";
        Console.WriteLine($"O novo prontuário será acrescentada na { rangeDestinoCirurgias }");
        await _sheetsDB.WriteRangeAsync(rangeDestinoCirurgias, bodyCirurgias.Values);
    }
    public async Task<Prontuario> AddProntuarioFromPdfAsync(string pacienteId, string pdfPath)
    {
        var paciente = await _iPacienteRepository.GetByIdAsync(pacienteId);

        if (paciente == null)
        {
            throw new Exception("Paciente não encontrado.");
        }
        var serviceExtractor = new ProntuarioPdfExtractorService(paciente);
        var prontuario = await serviceExtractor.ExtrairProntuarioDePdfAsync(pdfPath); 
        await AddProntuarioAsync(prontuario);
        return prontuario;
    }
    public async Task UpdateProntuarioAsync(Prontuario prontuario, string id)
    {
        Console.WriteLine("update method active");
        var prontuarioSheetraw = await _sheetsDB.LerRangeAsync("Prontuario!A2:AN"); // ou outro range total
        var prontuarioSheet = prontuarioSheetraw.ToList();
        int linhaIndexPront = prontuarioSheet.FindIndex(r => r.Count > 0 && r[35]?.ToString() == id); 
        if (linhaIndexPront == -1)throw new Exception("Prontuário não encontrado na aba Prontuario.");

        int linhaNoSheetPront = linhaIndexPront + 2; 
        Console.WriteLine($"line to be updated:{linhaNoSheetPront}");
       
        ValueRange bodyProntuario = ConstruirBodyProntuario(prontuario);
        // Console.WriteLine($"Testando instancia de prontuario: {descricao.NomePaciente}");
        string rangePront = $"Prontuario!A{linhaNoSheetPront}:AN{linhaNoSheetPront}";
        await _sheetsDB.WriteRangeAsync(rangePront, bodyProntuario.Values);

        // --- 2. Atualizar PEDIDOS DE EXAMES ---
        var examesSheetraw = await _sheetsDB.LerRangeAsync("PedidosExame!A2:D");
        var examesSheet = examesSheetraw.ToList();
        int linhaIndexExame = examesSheet.FindIndex(r => r.Count > 0 && r[3]?.ToString() == id); 

        if (linhaIndexExame != -1)
        {
            int linhaNoSheetExame = linhaIndexExame + 2;
            ValueRange bodyExames = ConstruirBodyExames(prontuario);
            string rangeExames = $"PedidosExame!A{linhaNoSheetExame}:D{linhaNoSheetExame}";
            await _sheetsDB.WriteRangeAsync(rangeExames, bodyExames.Values);
        }
        else if (prontuario.Exames != null && prontuario.Exames.Any())
        {
            // Inserir nova linha
            ValueRange bodyExames = ConstruirBodyExames(prontuario);
            await _sheetsDB.WriteRangeAsync("PedidosExame!A:D", bodyExames.Values); // Append insere no fim da planilha
        }

        // --- 3. Atualizar PEDIDOS DE CIRURGIA ---
        var cirurgiasSheetRaw = await _sheetsDB.LerRangeAsync("PedidosCirurgia!A2:P");
        var cirurgiasSheet = cirurgiasSheetRaw.ToList();
        int linhaIndexCirurgia = cirurgiasSheet.FindIndex(r => r.Count > 0 && r[15]?.ToString() == id);

        if (linhaIndexCirurgia != -1)
        {
            int linhaNoSheetCirurgia = linhaIndexCirurgia + 2;
            Console.WriteLine($"linha de Pedidoscirurgia a ser alterada: {linhaNoSheetCirurgia}");
            string procedimentosFormatados = string.Join("; ", prontuario.SolicitacaoInternacao.Procedimentos.Select(proc => $"{proc}"));
            ValueRange bodyCirurgias = ConstruirBodyCirurgias(prontuario);
            // Console.WriteLine($"valor da obs: {prontuario.SolicitacaoInternacao.Observacao}");
            string rangeCirurgias = $"PedidosCirurgia!A{linhaNoSheetCirurgia}:P{linhaNoSheetCirurgia}";
            await _sheetsDB.WriteRangeAsync(rangeCirurgias, bodyCirurgias.Values);
        }
        else if (prontuario.SolicitacaoInternacao?.Procedimentos != null && prontuario.SolicitacaoInternacao.Procedimentos.Any())
        {
            Console.WriteLine("Dentro do else");
            var pedidosCirurgiaSheet = await _sheetsDB.LerRangeAsync("PedidosCirurgia!A2:P");
            int novaLinhaCirurgiaIndex = pedidosCirurgiaSheet.Count(r => r.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString()))) + 2;

            string procedimentosFormatados = string.Join("; ", prontuario.SolicitacaoInternacao.Procedimentos.Select(proc => proc));
            ValueRange bodyCirurgias = ConstruirBodyCirurgias(prontuario);
            string rangeCirurgia = $"Prontuario!A{novaLinhaCirurgiaIndex}:AP{novaLinhaCirurgiaIndex}";
            await _sheetsDB.WriteRangeAsync(rangeCirurgia, bodyCirurgias.Values);
        }
    }
    public async Task DeleteProntuarioAsync(string id)
    {
        var prontuarioSheetraw = await _sheetsDB.LerRangeAsync("Prontuario!A2:AN"); // ou outro range total
        var prontuarioSheet = prontuarioSheetraw.ToList();
        int linhaIndexPront = prontuarioSheet.FindIndex(r => r.Count > 0 && r[35]?.ToString() == id); // Supondo que a coluna AJ (índice 35) seja o ID

        if (linhaIndexPront == -1)throw new Exception("Prontuário não encontrado na aba Prontuario.");
        // Passo 2: A linha no Google Sheets começa em 2 (1 para header)
        int linhaProntuarioNoSheet = linhaIndexPront + 2;
        await _sheetsDB.DeleteLineAsync(linhaProntuarioNoSheet, "Prontuario");
        Console.WriteLine($"A linha deletada será {linhaProntuarioNoSheet}");
        //exames
        var examesSheetraw = await _sheetsDB.LerRangeAsync("PedidosExame!A2:D");
        var examesSheet = examesSheetraw.ToList();
        int linhaIndexExame = examesSheet.FindIndex(r => r.Count > 0 && r[3]?.ToString() == id); 
        
        if (linhaIndexExame != -1)
        {
            int linhaNoSheetExame = linhaIndexExame + 2;
            await _sheetsDB.DeleteLineAsync(linhaNoSheetExame, "PedidosExame");
            Console.WriteLine($"A linha deletada será {linhaNoSheetExame}");
        }else Console.WriteLine("Não existe exame pare esse prontuário");
        //Cirurgia
        var cirurgiasSheetRaw = await _sheetsDB.LerRangeAsync("PedidosCirurgia!A2:P");
        var cirurgiasSheet = cirurgiasSheetRaw.ToList();
        int linhaIndexCirurgia = cirurgiasSheet.FindIndex(r => r.Count > 0 && r[15]?.ToString() == id);

        if (linhaIndexCirurgia != -1)
        {
            int linhaNoSheetCirurgia = linhaIndexCirurgia + 2;
            await _sheetsDB.DeleteLineAsync(linhaNoSheetCirurgia, "PedidosCirurgia");
            Console.WriteLine($"A linha deletada será {linhaNoSheetCirurgia}");
        }else Console.WriteLine("Não existe procedimento pare esse prontuário");
    }
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
            "Sem info" or "Sem Informação" => StatusVacinaHPV.SemInfo,
            _ => throw new ArgumentException($"Valor inválido para StatusVacinaHPV: '{valor}'")
        };
    }
    public static DateOnly? ParseDateOnly(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString) || dateString == "0" || dateString == "#N/A")
        {
            return null;
        }

        // Tentar fazer o parsing usando vários formatos possíveis
        // Use DateTime.ParseExact se você souber o formato exato
        // Use DateTime.TryParse se o formato puder variar ou for incerto
        DateOnly parsedDate;

        // Formato Brasileiro Comum: "dd/MM/yyyy"
        if (DateOnly.TryParseExact(dateString, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
        {
            // Console.WriteLine($"DEBUG: Data parseada com sucesso (dd/MM/yyyy): {parsedDate}");
            return parsedDate;
        }

        // Outro formato comum: "yyyy-MM-dd" (ISO)
        if (DateOnly.TryParseExact(dateString, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
        {
            // Console.WriteLine($"DEBUG: Data parseada com sucesso (yyyy-MM-dd): {parsedDate}");
            return parsedDate;
        }

        // Se o formato da sua planilha for algo diferente, adicione aqui.
        // Ex: Se for "dd-MM-yyyy"
        if (DateOnly.TryParseExact(dateString, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
        {
            // Console.WriteLine($"DEBUG: Data parseada com sucesso (dd-MM-yyyy): {parsedDate}");
            return parsedDate;
        }

        // Se tudo falhar, você pode registrar um erro ou retornar null
        Console.WriteLine($"ERRO: Falha ao parsear a data '{dateString}'. Formato inesperado ou inválido.");
        return null;
    }
    private ValueRange ConstruirBodyProntuario(Prontuario prontuario)
    {
        var descricao = prontuario.DescricaoBasica;
        var ago = prontuario.AGO;
        var ap = prontuario.Antecedentes;
        var af = prontuario.AntecedentesFamiliares;
        var posOp = prontuario.PosOperatorio;

        var displayVacina = ago?.VacinaHPV
            .GetType()
            .GetMember(ago.VacinaHPV.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            ?.Name ?? ago?.VacinaHPV.ToString();

        var acoes = prontuario.CD != null
            ? string.Join(", ", prontuario.CD.Select(cd =>
                cd.GetType()
                .GetMember(cd.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()?.Name ?? cd.ToString()))
            : "";

        var dataHoje =  DateOnly.FromDateTime(DateTime.Now).ToString("dd/MM/yyyy");
        
        ValueRange bodyProntuario = new()
        {
            Values = new List<IList<object>> {
                new List<object> {
                    string.IsNullOrWhiteSpace(descricao?.NomePaciente) ? "0" : descricao.NomePaciente,
                    string.IsNullOrWhiteSpace(descricao?.Cpf) ? "0" : descricao.Cpf,
                    descricao?.Idade ?? 0,              
                    string.IsNullOrWhiteSpace(descricao?.Profissao) ? "0" : descricao.Profissao,
                    string.IsNullOrWhiteSpace(descricao?.Religiao) ? "0" : descricao.Religiao,
                    string.IsNullOrWhiteSpace(descricao?.QD) ? "0" : descricao.QD,
                    string.IsNullOrWhiteSpace(descricao?.AtividadeFisica) ? "0" : descricao.AtividadeFisica,
                    string.IsNullOrWhiteSpace(ago.Menarca) ? "0" : ago.Menarca, 
                    string.IsNullOrWhiteSpace(ago.DUM) ? "0" : ago.DUM,
                    string.IsNullOrWhiteSpace(ago.Paridade) ? "0" : ago.Paridade,
                    string.IsNullOrWhiteSpace(ago.DesejoGestacao) ? "0" : ago.DesejoGestacao, // Se DesejoGestacao for um bool/enum, trate diferente
                    string.IsNullOrWhiteSpace(ago.Intercorrencias) ? "0" : ago.Intercorrencias,
                    string.IsNullOrWhiteSpace(ago.Amamentacao) ? "0" : ago.Amamentacao,
                    string.IsNullOrWhiteSpace(ago.VidaSexual) ? "0" : ago.VidaSexual,
                    string.IsNullOrWhiteSpace(ago.Relacionamento) ? "0" : ago.Relacionamento,
                    string.IsNullOrWhiteSpace(ago.Parceiros) ? "0" : ago.Parceiros,
                    string.IsNullOrWhiteSpace(ago.Coitarca) ? "0" : ago.Coitarca,
                    string.IsNullOrWhiteSpace(ago.IST) ? "0" : ago.IST,
                    displayVacina ?? "0",
                    string.IsNullOrWhiteSpace(ago.CCO) ? "0" : ago.CCO,
                    string.IsNullOrWhiteSpace(ago.MAC_TRH) ? "0" : ago.MAC_TRH,
                    string.IsNullOrWhiteSpace(ap?.Comorbidades) ? "0" : ap.Comorbidades,
                    string.IsNullOrWhiteSpace(ap?.Medicacao) ? "0" : ap.Medicacao,
                    string.IsNullOrWhiteSpace(ap?.Neoplasias) ? "0" : ap.Neoplasias,
                    string.IsNullOrWhiteSpace(ap?.Cirurgias) ? "0" : ap.Cirurgias,
                    string.IsNullOrWhiteSpace(ap?.Alergias) ? "0" : ap.Alergias,
                    string.IsNullOrWhiteSpace(ap?.Vicios) ? "0" : ap.Vicios,
                    string.IsNullOrWhiteSpace(ap?.HabitoIntestinal) ? "0" : ap.HabitoIntestinal,
                    string.IsNullOrWhiteSpace(ap?.Vacinas) ? "0" : ap.Vacinas,
                    string.IsNullOrWhiteSpace(af?.Neoplasias) ? "0" : af.Neoplasias,
                    string.IsNullOrWhiteSpace(af?.Comorbidades) ? "0" : af.Comorbidades,
                    string.IsNullOrWhiteSpace(prontuario.InformacoesExtras) ? "0" : prontuario.InformacoesExtras,
                    prontuario.DataConsulta?.ToString("dd/MM/yyyy") ?? dataHoje,
                    acoes ?? "0",
                    descricao.PacienteId ?? "0",
                    // string.IsNullOrWhiteSpace(prontuario.ID) ? "0" : prontuario.ID,
                    prontuario.ID,
                    prontuario.Tipo ?? "0",
                    string.IsNullOrWhiteSpace(posOp?.PeriodoSeguimento) ? "0" : posOp.PeriodoSeguimento,
                    string.IsNullOrWhiteSpace(posOp?.Conclusao) ? "0" : posOp.Conclusao,
                    string.IsNullOrWhiteSpace(posOp?.ExameMacro) ? "0" : posOp.ExameMacro,

                }
            }
        };
        return bodyProntuario;
    }
    private ValueRange ConstruirBodyExames(Prontuario prontuario)
    {
        var descricao = prontuario.DescricaoBasica;
        var dataHoje =  DateOnly.FromDateTime(DateTime.Now).ToString("dd/MM/yyyy");
        string examesFormatados = string.Join("; ", prontuario.Exames.Select(exame => $"{exame.Codigo} - {exame.Nome}"));
        return new ValueRange
        {
            Values = new List<IList<object>>
            {
                new List<object>
                {
                    string.IsNullOrWhiteSpace(descricao?.NomePaciente) ? "0" : descricao.NomePaciente,
                    prontuario.DataConsulta?.ToString("dd/MM/yyyy") ?? dataHoje,
                    string.IsNullOrWhiteSpace(examesFormatados) ? "0" : examesFormatados,
                    prontuario.ID ?? "0"
                }
            }
        };
    }
    private ValueRange ConstruirBodyCirurgias(Prontuario prontuario)
    {
        var descricao = prontuario.DescricaoBasica;
        var solicitacao = prontuario.SolicitacaoInternacao;
        string procedimentosFormatados = "0";
        var dataHoje =  DateOnly.FromDateTime(DateTime.Now).ToString("dd/MM/yyyy");
        if (solicitacao != null)
        {
            procedimentosFormatados = string.Join("; ", solicitacao.Procedimentos.Select(proc => $"{proc}"));
        }

        return new ValueRange
        {
            Values = new List<IList<object>>
            {
                new List<object>
                {
                    string.IsNullOrWhiteSpace(descricao?.NomePaciente) ? "0" : descricao.NomePaciente,
                    prontuario.DataConsulta?.ToString("dd/MM/yyyy") ?? dataHoje,
                    string.IsNullOrWhiteSpace(procedimentosFormatados) ? "0" :  procedimentosFormatados,
                    string.IsNullOrWhiteSpace(prontuario.SolicitacaoInternacao?.IndicacaoClinica) ? "0" : prontuario.SolicitacaoInternacao.IndicacaoClinica,
                    string.IsNullOrWhiteSpace(prontuario.SolicitacaoInternacao?.Observacao) ? "0" : prontuario.SolicitacaoInternacao.Observacao,
                    string.IsNullOrWhiteSpace(prontuario.SolicitacaoInternacao?.CID) ? "0" : prontuario.SolicitacaoInternacao.CID,
                    string.IsNullOrWhiteSpace(prontuario.SolicitacaoInternacao?.TempoDoenca) ? "0" : prontuario.SolicitacaoInternacao.TempoDoenca, // Tempo da doença - campo não mapeado
                    string.IsNullOrWhiteSpace(prontuario.SolicitacaoInternacao?.Diarias) ? "0" : prontuario.SolicitacaoInternacao.Diarias, // Diárias - campo não mapeado
                    string.IsNullOrWhiteSpace(prontuario.SolicitacaoInternacao?.Tipo) ? "0" : prontuario.SolicitacaoInternacao.Tipo,
                    string.IsNullOrWhiteSpace(prontuario.SolicitacaoInternacao?.Regime) ? "0" : prontuario.SolicitacaoInternacao.Regime,
                    string.IsNullOrWhiteSpace(prontuario.SolicitacaoInternacao?.Carater) ? "0" : prontuario.SolicitacaoInternacao.Carater,
                    prontuario.SolicitacaoInternacao.UsaOPME ? "Sim" : "Não",
                    string.IsNullOrWhiteSpace(prontuario.SolicitacaoInternacao?.Local) ? "0" : prontuario.SolicitacaoInternacao.Local,
                    string.IsNullOrWhiteSpace(prontuario.SolicitacaoInternacao?.Guia) ? "0" : prontuario.SolicitacaoInternacao.Guia, // Solicitação
                    string.IsNullOrWhiteSpace(prontuario.SolicitacaoInternacao?.Guia) ? "0" : prontuario.SolicitacaoInternacao.Guia, // Autorização
                    prontuario.ID ?? "0"
                }
            }
        };
    }
}


