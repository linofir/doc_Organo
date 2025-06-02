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
    public async Task<List<Prontuario>> GetProntuarioOfPacienteAsync( Paciente paciente )
    {
        return await CollectProntuariosOfPacienteAsync(paciente);
        // throw new NotImplementedException();
    }
    public async Task<Stream> CreateReportByIdAsync( string pacienteId )
    {
        var paciente = await  _iPacienteRepository.GetByIdAsync(pacienteId);
        if(paciente == null)
        {
            throw new InvalidOperationException($"Paciente com ID '{pacienteId}' não encontrado.");
        }
        var prontuariosOfPaciente = await GetProntuarioOfPacienteAsync( paciente);
        return _pdfGeneratorService.GeneratePatientReportPdf( paciente, prontuariosOfPaciente);
    }

    public async Task<Stream> CreateReportByCpfAsync( string pacienteCpf )
    {
        var paciente = await _iPacienteRepository.GetByCpfAsync(pacienteCpf);
        if(paciente == null)
        {
            throw new InvalidOperationException($"Paciente com CPF '{pacienteCpf}' não encontrado.");
        }
        var prontuariosOfPaciente = await GetProntuarioOfPacienteAsync( paciente);
        return _pdfGeneratorService.GeneratePatientReportPdf( paciente, prontuariosOfPaciente);
    }
    public async Task<List<Prontuario>> CollectProntuariosOfPacienteAsync(Paciente pacienteToCheck)
    {
        Console.WriteLine($"teste no collect paciente id: {pacienteToCheck.ID}");
        var prontuariosOfPaciente = new List<Prontuario>{};
        var prontuariosList = await GetProntuariosAsync();
        foreach (var prontuario in prontuariosList)
        {
            var pacienteId = prontuario.DescricaoBasica!.PacienteId;
            if(pacienteId == pacienteToCheck.ID)
            {
                prontuariosOfPaciente.Add(prontuario);
                Console.WriteLine($"paciente encontrado: {prontuario.DescricaoBasica.NomePaciente}");
            }
        }
        return prontuariosOfPaciente;
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
        var prontuarioSheetraw = await _sheetsDB.LerRangeAsync("Prontuario!A2:AJ"); // ou outro range total
        var prontuarioSheet = prontuarioSheetraw.ToList();
        int linhaIndexPront = prontuarioSheet.FindIndex(r => r.Count > 0 && r[35]?.ToString() == id); 
        if (linhaIndexPront == -1)throw new Exception("Prontuário não encontrado na aba Prontuario.");

        int linhaNoSheetPront = linhaIndexPront + 2; 
        Console.WriteLine($"line to be updated:{linhaNoSheetPront}");
        var descricao = prontuario.DescricaoBasica;
        var ago = prontuario.AGO;
        var ap = prontuario.Antecedentes;
        var af = prontuario.AntecedentesFamiliares;
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
        Console.WriteLine($"Testando instancia de prontuario: {descricao.NomePaciente}");
        string rangePront = $"Prontuario!A{linhaNoSheetPront}:AJ{linhaNoSheetPront}";
        await _sheetsDB.WriteRangeAsync(rangePront, bodyProntuario.Values);

        // --- 2. Atualizar PEDIDOS DE EXAMES ---
        var examesSheetraw = await _sheetsDB.LerRangeAsync("PedidosExame!A2:D");
        var examesSheet = examesSheetraw.ToList();
        int linhaIndexExame = examesSheet.FindIndex(r => r.Count > 0 && r[3]?.ToString() == id); 

        if (linhaIndexExame != -1)
        {
            int linhaNoSheetExame = linhaIndexExame + 2;
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
            string rangeExames = $"PedidosExame!A{linhaNoSheetExame}:D{linhaNoSheetExame}";
            await _sheetsDB.WriteRangeAsync(rangeExames, bodyExames.Values);
        }
        else if (prontuario.Exames != null && prontuario.Exames.Any())
        {
            // Inserir nova linha
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
            await _sheetsDB.WriteRangeAsync("PedidosExame!A:D", bodyExames.Values); // Append insere no fim da planilha
        }

        // --- 3. Atualizar PEDIDOS DE CIRURGIA ---
        var cirurgiasSheetRaw = await _sheetsDB.LerRangeAsync("PedidosCirurgia!A2:P");
        var cirurgiasSheet = cirurgiasSheetRaw.ToList();
        int linhaIndexCirurgia = cirurgiasSheet.FindIndex(r => r.Count > 0 && r[15]?.ToString() == id);

        if (linhaIndexCirurgia != -1)
        {
            int linhaNoSheetCirurgia = linhaIndexCirurgia + 2;
            Console.WriteLine($"linha ce cirurgia a ser alterada: {linhaNoSheetCirurgia}");
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
            Console.WriteLine($"valor da obs: {prontuario.SolicitacaoInternacao.Observacao}");
            string rangeCirurgias = $"PedidosCirurgia!A{linhaNoSheetCirurgia}:P{linhaNoSheetCirurgia}";
            await _sheetsDB.WriteRangeAsync(rangeCirurgias, bodyCirurgias.Values);
        }
        else if (prontuario.SolicitacaoInternacao?.Procedimentos != null && prontuario.SolicitacaoInternacao.Procedimentos.Any())
        {
            Console.WriteLine("Dentro do else");
            var pedidosCirurgiaSheet = await _sheetsDB.LerRangeAsync("PedidosCirurgia!A2:P");
            int novaLinhaCirurgiaIndex = pedidosCirurgiaSheet.Count(r => r.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString()))) + 2;

            string procedimentosFormatados = string.Join("; ", prontuario.SolicitacaoInternacao.Procedimentos.Select(proc => proc));
            ValueRange bodyCirurgias = new()
            {
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
                        prontuario.SolicitacaoInternacao.TempoDoenca,
                        prontuario.SolicitacaoInternacao.Diarias,
                        prontuario.SolicitacaoInternacao.Tipo,
                        prontuario.SolicitacaoInternacao.Regime,
                        prontuario.SolicitacaoInternacao.Carater,
                        prontuario.SolicitacaoInternacao.UsaOPME ? "Sim" : "Não",
                        prontuario.SolicitacaoInternacao.Local,
                        prontuario.SolicitacaoInternacao.Guia,
                        prontuario.SolicitacaoInternacao.Guia,
                        prontuario.ID
                    }
                }
            };
            string rangeCirurgia = $"Prontuario!A{novaLinhaCirurgiaIndex}:AJ{novaLinhaCirurgiaIndex}";
            await _sheetsDB.WriteRangeAsync(rangeCirurgia, bodyCirurgias.Values);
        }
    }
    public async Task DeleteProntuarioAsync(string id)
    {
        var prontuarioSheetraw = await _sheetsDB.LerRangeAsync("Prontuario!A2:AJ"); // ou outro range total
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
            "Sem info" or "Sem informação" => StatusVacinaHPV.SemInfo,
            _ => throw new ArgumentException($"Valor inválido para StatusVacinaHPV: '{valor}'")
        };
    }
    //public async Task UpdateProntuarioAsync(Prontuario prontuario, string id)
    // {
    //     // 1. Buscar as planilhas
    //     var prontuarioSheet = await _sheetsDB.LerRangeAsync("Prontuario!A2:AJ");
    //     var pedidosExameSheet = await _sheetsDB.LerRangeAsync("PedidosExame!A2:D");
    //     var pedidosCirurgiaSheet = await _sheetsDB.LerRangeAsync("PedidosCirurgia!A2:P");

    //     // 2. Descobrir índices das linhas com base no ID
    //     int linhaProntuario = EncontrarLinhaPorId(prontuarioSheet, id) + 2;
    //     int linhaExame = EncontrarLinhaPorId(pedidosExameSheet, id) + 2;
    //     int linhaCirurgia = EncontrarLinhaPorId(pedidosCirurgiaSheet, id) + 2;

    //     if (linhaProntuario < 2 || linhaExame < 2 || linhaCirurgia < 2)
    //         throw new Exception("ID de prontuário não encontrado em uma das planilhas.");

    //     // 3. Preparar os valores formatados
    //     var paciente = await _iPacienteRepository.GetByIdAsync(prontuario.DescricaoBasica.PacienteId);
    //     if (paciente == null)
    //         throw new Exception("Paciente não encontrado.");

    //     var displayVacina = prontuario.AGO.VacinaHPV
    //         .GetType()
    //         .GetMember(prontuario.AGO.VacinaHPV.ToString())
    //         .First()
    //         .GetCustomAttribute<DisplayAttribute>()?.Name ?? prontuario.AGO.VacinaHPV.ToString();

    //     var acoes = prontuario.CD != null
    //         ? string.Join(", ", prontuario.CD.Select(cd =>
    //             cd.GetType()
    //             .GetMember(cd.ToString())
    //             .First()
    //             .GetCustomAttribute<DisplayAttribute>()?.Name ?? cd.ToString()))
    //         : "";

    //     var dataHoje = DateTime.Now.ToString("dd/MM/yyyy");

    //     // 4. Montar ValueRange para cada aba
    //     var bodyProntuario = ConstruirBodyProntuario(prontuario, paciente, displayVacina, dataHoje, acoes);
    //     var bodyExames = ConstruirBodyExames(prontuario, dataHoje);
    //     var bodyCirurgias = ConstruirBodyCirurgias(prontuario, dataHoje);

    //     // 5. Escrever os dados nas respectivas linhas
    //     await _sheetsDB.WriteRangeAsync($"Prontuario!A{linhaProntuario}:AJ{linhaProntuario}", bodyProntuario.Values);
    //     await _sheetsDB.WriteRangeAsync($"PedidosExame!A{linhaExame}:D{linhaExame}", bodyExames.Values);
    //     await _sheetsDB.WriteRangeAsync($"PedidosCirurgia!A{linhaCirurgia}:P{linhaCirurgia}", bodyCirurgias.Values);
    // }
    private ValueRange ConstruirBodyProntuario(Prontuario prontuario)
    {
        var descricao = prontuario.DescricaoBasica;
        var ago = prontuario.AGO;
        var ap = prontuario.Antecedentes;
        var af = prontuario.AntecedentesFamiliares;

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

        return new ValueRange
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
    }
    private ValueRange ConstruirBodyExames(Prontuario prontuario)
    {
        var descricao = prontuario.DescricaoBasica;

        string examesFormatados = string.Join("; ", prontuario.Exames.Select(exame => $"{exame.Codigo} - {exame.Nome}"));

        return new ValueRange
        {
            Values = new List<IList<object>> {
                new List<object> {
                    descricao.NomePaciente,
                    prontuario.DataRequisicao.ToString("dd/MM/yyyy"),
                    examesFormatados,
                    prontuario.ID
                }
            }
        };
    }
    private ValueRange ConstruirBodyCirurgias(Prontuario prontuario, string dataHoje)
    {
        var descricao = prontuario.DescricaoBasica;
        var solicitacao = prontuario.SolicitacaoInternacao;

        string procedimentosFormatados = string.Join("; ", solicitacao.Procedimentos.Select(p => p.ToString()));

        return new ValueRange
        {
            Values = new List<IList<object>> {
                new List<object> {
                    descricao.NomePaciente,
                    prontuario.DataRequisicao.ToString("dd/MM/yyyy"),
                    procedimentosFormatados,
                    solicitacao.IndicacaoClinica,
                    solicitacao.Observacao,
                    solicitacao.CID,
                    solicitacao.TempoDoenca,
                    solicitacao.Diarias,
                    solicitacao.Tipo,
                    solicitacao.Regime,
                    solicitacao.Carater,
                    solicitacao.UsaOPME ? "Sim" : "Não",
                    solicitacao.Local,
                    solicitacao.Guia, // Solicitação
                    solicitacao.Guia, // Autorização
                    prontuario.ID
                }
            }
        };
    }
}


