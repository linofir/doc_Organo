using System.ComponentModel.DataAnnotations;
using DocAPI.Core.Models;
using DocAPI.Core.Repositories;
using DocAPI.Services;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Sheets.v4.Data;
using NPOI.SS.Formula.Functions;
// using DocAPI.Data.Dtos.Atendimento;   

namespace DocAPI.Infrastructure.SheetsDb;
public class AtendimentoSheetsRepository : IAtendimentoRepository
{
    private readonly GoogleSheetsDB _sheetsDB;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IProntuarioRepository _prontuarioRepository;
    private readonly IAgendamentoRepository _agendamentoRepository; // Se precisar de agendamento
    private readonly PdfGeneratorService _pdfGeneratorService;
    private readonly FileDataOfSenhaExtractorService _fileDataOfSenhaExtractorService;


    public AtendimentoSheetsRepository(IPacienteRepository pacienteRepository,
                         IProntuarioRepository prontuarioRepository,
                         IAgendamentoRepository agendamentoRepository,
                         PdfGeneratorService pdfGeneratorService,
                         FileDataOfSenhaExtractorService fileDataOfSenhaExtractorService,
                         GoogleSheetsDB sheetsDB)
    {
        _pacienteRepository = pacienteRepository;
        _prontuarioRepository = prontuarioRepository;
        _agendamentoRepository = agendamentoRepository;
        _pdfGeneratorService = pdfGeneratorService;
        _fileDataOfSenhaExtractorService = fileDataOfSenhaExtractorService;
        _sheetsDB = sheetsDB;
    }
    public async Task<IEnumerable<Atendimento>> GetAllAsync(int skip = 0, int take = 10)
    {
        var atendimentos = await GetAtendimentosAsync();
        return atendimentos.Skip(skip).Take(take);
    }
    public async Task<Atendimento?> GetByIdAsync(string id)
    {
        var atendimentos = await GetAtendimentosAsync();
        return atendimentos.FirstOrDefault( a => a.ID == id);
    }
    public async Task CreateAsync(Atendimento novoAtendimento)
    {
       await AddAtendimentoAsync(novoAtendimento);
    }
    public async Task UpdateAsync(Atendimento atendimento, string id)
    {
       await UpdateAtendimentoAsync(atendimento, id);
    }
    public async Task DeleteAsync(string id)
    {
       await DeleteAtendimentooAsync(id);
    }
    public async Task<Stream> CreateReportByIdAsync( string pacienteId )
    {
        return await GeneratePatientPdfFullReport(pacienteId);
    }
    public async Task<Atendimento> CreateReportFollwUpByIdAsync( string pacienteId )
    {
        // return await Atendimento(pacienteId);
        throw new NotImplementedException();
    }

    // métodos auxiliares//////////////
    
    public async Task AddAtendimentoAsync(Atendimento atendimento)
    {
        // Definir quais são os dados a serem persisitidos no DB, desconsiderando os já existentes em outras planilhas.
        Console.WriteLine($"Comunicando com DB..." );
        var atendimentoSheets = await _sheetsDB.LerRangeAsync("Atendimento!A3:AL");//definr tabela
        int novaLinhaIndex = atendimentoSheets.Count(r => r.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString()))) + 3;
        //Como é melhor armazenar o status, é preciso validar e preparar os dados antes de armazena-los
        if (string.IsNullOrEmpty(atendimento.ID))
        {
            atendimento.ID = Guid.NewGuid().ToString();
        };


        ValueRange body = CreateAtendimentoSheets(atendimento);
        
        // 3. Escrever os dados na próxima linha disponível
        string rangeDestino = $"Atendimento!A{novaLinhaIndex}:AL{novaLinhaIndex}";
        Console.WriteLine($"O novo Atendimento será acrescentado na { rangeDestino}");
        await _sheetsDB.WriteRangeAsync(rangeDestino, body.Values);
    }
    private ValueRange CreateAtendimentoSheets(Atendimento atendimento)
    {
        var eC = atendimento.EtapaConsulta;
        var ePreP = atendimento.EtapaPreProcedimento;
        var eProc = atendimento.EtapaProcedimento;
        var ePosP = atendimento.EtapaPosProcedimento;
//fazer cruzamento de dados já existentes com os novos dados
        var prontuariosFormatados = "";
        var agendamentosFormatados = "";
        var cdFormatados = "";
        var procedimentosFormatados = "";
        if (atendimento != null)
        {
            // prontuariosFormatados = string.Join("; ", atendimento.ProntuariosId!.Select(proc => $"{proc}"));
            // agendamentosFormatados = string.Join("; ", atendimento.AgendamentosId!.Select(proc => $"{proc}"));
            // cdFormatados = string.Join("; ", eC.CdPendente!.Select(proc => $"{proc}"));
            // procedimentosFormatados = string.Join("; ", ePreP.Procedimentos!.Select(proc => $"{proc}"));
            
            prontuariosFormatados = atendimento?.ProntuariosId?.Any() == true
                ? string.Join(", ", atendimento.ProntuariosId): "0";

            agendamentosFormatados = atendimento?.AgendamentosId != null 
                ? string.Join(", ", atendimento.AgendamentosId) 
                : "0";

            cdFormatados = eC?.CdPendente != null 
                ? string.Join(", ", eC.CdPendente.Select(cd => $"{cd.Descricao} - {cd.Pendente.ToString()}"))
                : "0";

            procedimentosFormatados = ePreP?.Procedimentos != null 
                ? string.Join(", ", ePreP.Procedimentos)
                : "0";

        }else
        {
            prontuariosFormatados = "teste else";
            agendamentosFormatados = "teste else";
            cdFormatados = "teste else";
            procedimentosFormatados = "teste else";
        }

        // var displayVacina = ago?.VacinaHPV
        //     .GetType()
        //     .GetMember(ago.VacinaHPV.ToString())
        //     .First()
        //     .GetCustomAttribute<DisplayAttribute>()
        //     ?.Name ?? ago?.VacinaHPV.ToString();

        // var acoes = prontuario.CD != null
        //     ? string.Join(", ", prontuario.CD.Select(cd =>
        //         cd.GetType()
        //         .GetMember(cd.ToString())
        //         .First()
        //         .GetCustomAttribute<DisplayAttribute>()?.Name ?? cd.ToString()))
        //     : "";

        // var dataHoje =  DateOnly.FromDateTime(DateTime.Now).ToString("dd/MM/yyyy");
        
        ValueRange bodyAtendimento = new()
        {
            Values = new List<IList<object>> {
                new List<object> {
                    string.IsNullOrWhiteSpace(atendimento?.ID) ? "0" : atendimento.ID,
                    string.IsNullOrWhiteSpace(atendimento?.PacienteId) ? "0" : atendimento.PacienteId,
                    string.IsNullOrWhiteSpace(atendimento?.NomePaciente) ? "0" : atendimento.NomePaciente,
                    string.IsNullOrWhiteSpace(prontuariosFormatados) ? "0" : prontuariosFormatados,
                    string.IsNullOrWhiteSpace(agendamentosFormatados) ? "0" : agendamentosFormatados,
                    string.IsNullOrWhiteSpace(atendimento?.EtapaAtualAtendimento) ? "0" : atendimento.EtapaAtualAtendimento,
                    string.IsNullOrWhiteSpace(atendimento?.MensagemParaMedico) ? "0" : atendimento.MensagemParaMedico,
                    //EtapaConsulta
                    string.IsNullOrWhiteSpace(eC?.ProntuarioIdVigente) ? "0" : eC?.ProntuarioIdVigente,
                    string.IsNullOrWhiteSpace(eC?.StatusGeral) ? "0" : eC?.StatusGeral!,
                    string.IsNullOrWhiteSpace(eC?.CadastroConfirmado) ? "0" : eC?.CadastroConfirmado!,
                    string.IsNullOrWhiteSpace(eC?.ConsultaConcluida) ? "0" : eC?.ConsultaConcluida!,
                    string.IsNullOrWhiteSpace(eC?.DataConsultaConcluida?.ToString("dd/MM/yyyy")) ? "0" : eC?.DataConsultaConcluida?.ToString("dd/MM/yyyy")!, 
                    string.IsNullOrWhiteSpace(cdFormatados) ? "0" : cdFormatados,
                    //EtapaPreProcedimento
                    string.IsNullOrWhiteSpace(ePreP?.AgendamentoIdVigente) ? "0" : ePreP?.AgendamentoIdVigente,
                    string.IsNullOrWhiteSpace(ePreP?.StatusAgendamento) ? "0" : ePreP?.StatusAgendamento!,
                    string.IsNullOrWhiteSpace(ePreP?.DataAgendamento?.ToString("dd/MM/yyyy")) ? "0" : ePreP?.DataAgendamento?.ToString("dd/MM/yyyy")!,
                    string.IsNullOrWhiteSpace(procedimentosFormatados) ? "0" : procedimentosFormatados,
                    string.IsNullOrWhiteSpace(ePreP?.StatusGeral) ? "0" : ePreP?.StatusGeral!,
                    string.IsNullOrWhiteSpace(ePreP?.UltimaAtualizacaoSenhas?.ToString("dd/MM/yyyy HH:mm")) ? "0" : ePreP?.UltimaAtualizacaoSenhas?.ToString("dd/MM/yyyy HH:mm")!,
                    string.IsNullOrWhiteSpace(ePreP?.StatusSenha) ? "0" : ePreP?.StatusSenha!,
                    string.IsNullOrWhiteSpace(ePreP?.StatusTermoCirurgico) ? "0" : ePreP?.StatusTermoCirurgico!,
                    string.IsNullOrWhiteSpace(ePreP?.StatusEncaminhamento) ? "0" : ePreP?.StatusEncaminhamento!,
                    string.IsNullOrWhiteSpace(ePreP?.StatusInstrumentadora) ? "0" : ePreP?.StatusInstrumentadora!,
                    string.IsNullOrWhiteSpace(ePreP?.StatusExames) ? "0" : ePreP?.StatusExames!,
                    //EtapaProcedimento
                    string.IsNullOrWhiteSpace(eProc?.StatusProcedimento) ? "0" : eProc?.StatusProcedimento!,
                    string.IsNullOrWhiteSpace(eProc?.StatusGeral) ? "0" : eProc?.StatusGeral!,
                    string.IsNullOrWhiteSpace(eProc?.StatusAtestado) ? "0" : eProc?.StatusAtestado!,
                    string.IsNullOrWhiteSpace(eProc?.StatusInstrucoes) ? "0" : eProc?.StatusInstrucoes!,
                    string.IsNullOrWhiteSpace(eProc?.StatusConsulta) ? "0" : eProc?.StatusConsulta!,
                    string.IsNullOrWhiteSpace(eProc?.dataConsultaPosOp.ToString("dd/MM/yyyy")) ? "0" : eProc?.dataConsultaPosOp.ToString("dd/MM/yyyy")!,
                    //EtapaPosProcedimento
                    string.IsNullOrWhiteSpace(ePosP?.StatusConsultaPosOp) ? "0" : ePosP?.StatusConsultaPosOp!,
                    string.IsNullOrWhiteSpace(ePosP?.AgendamentoPosOp.ToString("dd/MM/yyyy")) ? "0" : ePosP?.AgendamentoPosOp.ToString("dd/MM/yyyy")!,
                    string.IsNullOrWhiteSpace(ePosP?.ProntuarioPosOpId) ? "0" : ePosP?.ProntuarioPosOpId!,
                    string.IsNullOrWhiteSpace(ePosP?.StatusRecomendacoesMedicas) ? "0" : ePosP?.StatusRecomendacoesMedicas!,
                    string.IsNullOrWhiteSpace(ePosP?.StatusSeguimento) ? "0" : ePosP?.StatusSeguimento!,
                    string.IsNullOrWhiteSpace(ePosP?.PrevisaoSeguimento.ToString("dd/MM/yyyy")) ? "0" : ePosP?.PrevisaoSeguimento.ToString("dd/MM/yyyy")!,
                    string.IsNullOrWhiteSpace(ePosP?.AlarmeSeguimento.ToString("dd/MM/yyyy")) ? "0" : ePosP?.AlarmeSeguimento.ToString("dd/MM/yyyy")!,
                    string.IsNullOrWhiteSpace(ePosP?.StatusGeral) ? "0" : ePosP?.StatusGeral!,
                }
            }
        };
        return bodyAtendimento;
    }

    public async Task<List<Atendimento>> GetAtendimentosAsync()
    {
        var values = await _sheetsDB.LerRangeAsync("Atendimento!A3:AL"); // de A até a coluna ID
        var allAtendimentos = new List<Atendimento>();
        var limit = values.Count;
        // Console.WriteLine($"Total de linhas com algum dado: {limit}");       
        for (int i = 0; i < limit; i++)
        {
            var row = values[i];
            if (row.All(cell => string.IsNullOrWhiteSpace(cell?.ToString()))) continue;
            //Confere se tem alguma coluna vazia
            if (row.Count < 38)
            {
                Console.WriteLine($"Linha {i + 3} ignorada: colunas insuficientes ({row.Count}).");
                continue;
            }
            try
            {
                var atendimento = await CollectAtendimento(row);
                allAtendimentos.Add(atendimento);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar linha {i + 3}: {ex.Message}");
            }
        }
        return allAtendimentos;
    }
    public async Task<Atendimento> CollectAtendimento(IList<object> row)
    {
        // Console.WriteLine("teste collect");
        //Coletar dados do DB
        var atendimento = new Atendimento()
        {
            ID = row[0].ToString() ?? "",
            PacienteId = row[1].ToString() ?? "",
            NomePaciente = row[2].ToString() ?? "",
            ProntuariosId = row[3]!.ToString()!.Split(',').ToList(),
            AgendamentosId = row[4]!.ToString()!.Split(',').ToList(),
            EtapaAtualAtendimento = row[5].ToString() ?? "",
            MensagemParaMedico = row[6].ToString() ?? "",
            EtapaConsulta = new ConsultaEtapaStatus()
            {
                ProntuarioIdVigente = row[7]!.ToString() ?? "",//definir como se já coleto o prontuário ou deino somento o ID
                StatusGeral = row[8]!.ToString() ?? "",
                CadastroConfirmado = row[9]!.ToString() ?? "",
                ConsultaConcluida = row[10]!.ToString() ?? "",
                DataConsultaConcluida = ParseDateOnly(row[11]!.ToString()),
                CdPendente = ParseCdPendente(row[12].ToString())// Verificar como será o armazrnamento row[12]
            },
            EtapaPreProcedimento = new PreProcedimentoEtapaStatus()
            {
                AgendamentoIdVigente = row[7]!.ToString() ?? "",
                StatusAgendamento = row[14]!.ToString() ?? "",
                DataAgendamento = ParseDateOnly(row[15]!.ToString()!),
                Procedimentos = row[16]!.ToString()!.Split(',').ToList(),
                StatusGeral = row[17]!.ToString() ?? "",
                UltimaAtualizacaoSenhas = ParseDateTime(row[17]!.ToString()!),
                StatusSenha = row[19]!.ToString() ?? "",
                StatusTermoCirurgico = row[20]!.ToString() ?? "",
                StatusEncaminhamento = row[21]!.ToString() ?? "",
                StatusInstrumentadora = row[22]!.ToString() ?? "",
                StatusExames = row[23]!.ToString() ?? ""
            },
            EtapaProcedimento = new ProcedimentoEtapaStatus()
            {
                StatusProcedimento = row[24]!.ToString() ?? "",
                StatusGeral = row[25]!.ToString() ?? "",
                StatusAtestado = row[26]!.ToString() ?? "",
                StatusInstrucoes = row[27]!.ToString() ?? "",
                StatusConsulta = row[28]!.ToString() ?? "",
                dataConsultaPosOp = ParseDateOnly(row[29]!.ToString()!),
            },
            EtapaPosProcedimento = new PosProcedimentoEtapaStatus()
            {
                StatusConsultaPosOp = row[30]!.ToString() ?? "",
                AgendamentoPosOp = ParseDateOnly(row[31]!.ToString()!),
                ProntuarioPosOpId = row[32]!.ToString() ?? "",
                StatusRecomendacoesMedicas = row[33]!.ToString() ?? "",
                StatusSeguimento = row[34]!.ToString() ?? "",
                PrevisaoSeguimento = ParseDateOnly(row[35]!.ToString()!),
                AlarmeSeguimento = ParseDateOnly(row[36]!.ToString()!),
                StatusGeral = row[37]!.ToString() ?? "",
            }
        };
        // var pacienteId = row[1].ToString();

        // dados a serem validados para cada paciente
        // if(!string.IsNullOrWhiteSpace(atendimento.PacienteId))
        // {
        //     atendimento = await AtualizarAtendimento(pacienteId, atendimento);
        // }
        return atendimento;
    }
    public async Task UpdateAtendimentoAsync(Atendimento atendimento, string id)
    {
        Console.WriteLine("update method active");
        var atendimentoSheetraw = await _sheetsDB.LerRangeAsync("Atendimento!A3:AL"); // ou outro range total
        var atendimentoSheet = atendimentoSheetraw.ToList();
        int linhaIndexAtend = atendimentoSheet.FindIndex(r => r.Count > 0 && r[0]?.ToString() == id); 
        if (linhaIndexAtend == -1)throw new Exception("Atendimento não encontrado na aba Atendimento.");

        int linhaNoSheetAtend = linhaIndexAtend + 3; 
        Console.WriteLine($"line to be updated:{linhaNoSheetAtend}");
        atendimento.ID = id;
        Console.WriteLine($"Testando instancia de atendimento: {atendimento.ID}");
        ValueRange bodyAtendimento = CreateAtendimentoSheets(atendimento);
        string rangePront = $"Atendimento!A{linhaNoSheetAtend}:AL{linhaNoSheetAtend}";
        await _sheetsDB.WriteRangeAsync(rangePront, bodyAtendimento.Values);

    }

    public async Task DeleteAtendimentooAsync(string id)
    {
        var atendimentoSheetraw = await _sheetsDB.LerRangeAsync("Atendimento!A3:AL"); // ou outro range total
        var atendimentoSheet = atendimentoSheetraw.ToList();
        int linhaIndexPront = atendimentoSheet.FindIndex(r => r.Count > 0 && r[0]?.ToString() == id); // Supondo que a coluna AL (índice 0) seja o ID

        if (linhaIndexPront == -1)throw new Exception("Atendiento não encontrado na aba Atendimento.");
        // Passo 2: A linha no Google Sheets começa em 2 (1 para header)
        int linhaProntuarioNoSheet = linhaIndexPront + 3;
        await _sheetsDB.DeleteLineAsync(linhaProntuarioNoSheet, "Atendimento");
        Console.WriteLine($"A linha deletada será {linhaProntuarioNoSheet}");
        
    }
    //Atualizar atendimento
    public async Task<Atendimento> AtualizarAtendimento ( string pacienteId, Atendimento atendimento)
    {
        // Criar lógicas para definirem a etapa do atendimento, inicializando.
        
        // if (string.IsNullOrEmpty(atendimento.ID))
        // {
        //     atendimento.ID = Guid.NewGuid().ToString();
        // }
        // Para Etapa COnsulta, Coletar Paciente
        var paciente = await _pacienteRepository.GetByIdAsync(pacienteId);
        // var prontuariosOfPaciente = new List<Prontuario>();
        // Coletando prontuarios de paciente
        if(paciente != null)
        {
            // prontuariosOfPaciente = await _prontuarioRepository.GetProntuariosOfPacienteAsync(paciente);
            atendimento = await ValidacaoEtapaConsulta(paciente , atendimento);
        }
        //Para Etapa Pré Procedimento
        // Coletar agendamentos
        var agendamentosOfPaciente = await _agendamentoRepository.GetByPacienteIdAsync(pacienteId);
        atendimento = await ValidacaoPreProcedimento(atendimento, agendamentosOfPaciente, paciente);
        
        // Procedimento
        atendimento.EtapaProcedimento.StatusGeral = "Procedimento Pendente";
        // Dados oriundos do DB, procedimento, instruções, atestado, dataConsulta
        atendimento = ValidacaoEtapaProcedimento(atendimento, agendamentosOfPaciente, paciente);
        

        // Pós Procedimento
        // Data Consulta pós OP
        // Identificar prontuario de pos op
        // Alarme para Seguimento médico.

        return atendimento;
    }
    public async Task<Atendimento> ValidacaoEtapaConsulta(/*string pacienteId,*/ Paciente paciente, Atendimento atendimento /*,List<Prontuario> prontuariosOfPaciente*/)
    {
        if(paciente == null)
        {
            // throw new InvalidOperationException($"Paciente com ID '{pacienteId}' não encontrado.");
            atendimento.EtapaConsulta!.CadastroConfirmado = "false";
            atendimento.MensagemParaMedico = $"Problema ao identificar o cadastro da paciente com ID: {atendimento.PacienteId}";
            return atendimento;
        }
        // Cadastro de paciente confirmado
        // atendimento.PacienteId = pacienteId;
        atendimento.EtapaConsulta!.CadastroConfirmado = "true";
        atendimento.EtapaConsulta.StatusGeral = "Consulta Pendente";
        atendimento.EtapaAtualAtendimento = "Inicialização";

        var allAcoesCD = Enum.GetValues(typeof(AcoesCD))
                             .Cast<AcoesCD>()
                             .Select(action => new
                             {
                                 Value = action,
                                 DisplayName = GetEnumDisplayName(action)
                             })
                             .ToList();
        //Estou aqui/////////////////////////////////////////////////
        var prontuariosId = new List<string>(){};
        var prontuariosOfPaciente = await _prontuarioRepository.GetProntuariosOfPacienteAsync(atendimento.PacienteId);//Conferir se esse é o paciente id a ser utilizado
        // var procedimentos = new List<string>();
        
        // Console.WriteLine($"quantidade de prontuários encontrados no atendimento: {prontuariosOfPaciente.Count()}");
        if(prontuariosOfPaciente != null && prontuariosOfPaciente.Any())
        {
            // Delimitando por prontuarios do Tipo Consulta(que inicia um atendimento), coleta o ultimo prontuário, ou algum já selecionado
            var prontuarioSolicitacao = new Prontuario(){DataConsulta = DateOnly.MinValue};
            foreach (var p in prontuariosOfPaciente)
            {
                if(p.Tipo == "Consulta")
                {
                    prontuariosId.Add(p.ID);
                    prontuarioSolicitacao = p.DataConsulta > prontuarioSolicitacao.DataConsulta ? p : prontuarioSolicitacao;
                    if(atendimento.EtapaConsulta.ProntuarioConsulta == null)
                    {
                        atendimento.EtapaConsulta.ProntuarioConsulta = prontuarioSolicitacao;
                    }
                }
            }
            atendimento.ProntuariosId = prontuariosId;
            atendimento.EtapaConsulta.DataConsultaConcluida = atendimento.EtapaConsulta.ProntuarioConsulta.DataConsulta;
            atendimento.EtapaPreProcedimento.Procedimentos = atendimento.EtapaConsulta.ProntuarioConsulta.SolicitacaoInternacao.Procedimentos;
            // Console.WriteLine($"Valor da data : {atendimento.EtapaConsulta.DataConsultaConcluida}");
            atendimento.EtapaAtualAtendimento = "Consulta";
            
            if(atendimento.EtapaConsulta.DataConsultaConcluida <= DateOnly.FromDateTime(DateTime.Today)) 
            {
                atendimento.EtapaConsulta.ConsultaConcluida = "true";
                // atendimento.EtapaConsulta.StatusGeral = "Consulta Realizada";
                atendimento.MensagemParaMedico = $"Atendimento aberto da paciente {paciente.Nome}";
            }
            // Fornecer CDs geradas na consulta e status de cada CD
            var cdStatusList = new List<CDStatus>();

            // Iterar sobre todas as ações CD possíveis
            foreach (var action in allAcoesCD)
            {
                if (prontuarioSolicitacao.CD != null && prontuarioSolicitacao.CD.Contains(AcoesCD.SemInformacao) && prontuarioSolicitacao.CD.Count == 1)
                {
                    // Se "Sem Info" é a única CD, então todas as outras são pendentes.
                    // Populate cdStatusList marcando tudo como Pendente, exceto SemInformacao.
                    // ... (implementar essa lógica)
                    atendimento.MensagemParaMedico = $"Atendimento aberto da paciente {paciente.Nome}, com registro de 'Sem Info' nas CDs.";
                    // Pode ser um bom ponto para retornar ou definir um status geral específico.
                    return atendimento;
                }
                // Verificar se a ação está presente na lista CD do prontuário
                var isPresent = prontuarioSolicitacao.CD?.Contains(action.Value) ?? false;

                cdStatusList.Add(new CDStatus
                {
                    Descricao = action.DisplayName,
                    Pendente = !isPresent // Se não estiver presente, está pendente
                });
            }
            atendimento.EtapaConsulta.CdPendente = cdStatusList;
            //Validação de Cds
            foreach (var cd in cdStatusList)
            {
                if(cd.Pendente)
                {
                    atendimento.MensagemParaMedico = $"Atendimento aberto da paciente {paciente.Nome}, mas existem pendências CD. Atualizar para continuar o atendimento.";
                    return atendimento;
                }
            }
        }else
        {
            atendimento.MensagemParaMedico = $"Nenhum Prontuário encontrado para a paciente {paciente.Nome}.";
        }
        atendimento.EtapaConsulta.StatusGeral = "Consulta Concluída";
        atendimento.EtapaAtualAtendimento = "Pré Procedimento";
        return atendimento;
    }
    public async Task<Atendimento> ValidacaoPreProcedimento(Atendimento atendimento, List<Agendamento> agendamentosOfPaciente, Paciente paciente)
    {
        atendimento.EtapaPreProcedimento.StatusGeral = "PreOp pendente";
    //informações do DB, da tabela Atendimento
        // atendimento.EtapaPreProcedimento.StatusEncaminhamento = "true";
        // atendimento.EtapaPreProcedimento.StatusExames = "true";
        // atendimento.EtapaPreProcedimento.StatusInstrumentadora = "true";
        // atendimento.EtapaPreProcedimento.StatusTermoCirurgico = "true";
        var agendamentosId = new List<string>(){};
        var agendamentoTeste = new Agendamento(){Data = DateOnly.MinValue};
        if(agendamentosOfPaciente != null && agendamentosOfPaciente.Any())
        {
           foreach (var a in agendamentosOfPaciente)
            {
                agendamentosId.Add(a.ID);
                agendamentoTeste = a.Data > agendamentoTeste.Data ? a : agendamentoTeste;// será o ultimo agendamento realizado caso já não exista um agendamento selecionado , definir melhor
                if(atendimento.EtapaPreProcedimento.AgendamentoProcedimento == null)
                {
                    atendimento.EtapaPreProcedimento.AgendamentoProcedimento = agendamentoTeste;
                }
            }
            atendimento.AgendamentosId = agendamentosId;
            // agendamentoTeste = agendamentosOfPaciente[0];
            atendimento.EtapaPreProcedimento.DataAgendamento = atendimento.EtapaPreProcedimento.AgendamentoProcedimento?.Data;
            atendimento.EtapaPreProcedimento.StatusAgendamento = "true";
            // atendimento.MensagemParaMedico = $"Nenhum Prontuário encontrado para a paciente {paciente.Nome}.";
            // if(atendimento.EtapaPreProcedimento.DataAgendamento <= DateOnly.FromDateTime(DateTime.Today)) 
            // {
            //     // atendimento.EtapaConsulta.StatusGeral = "Consulta Realizada";
            //     atendimento.MensagemParaMedico = $"Checar se o procedimento da paciente: {paciente.Nome}";
            // }
        }else
        {
        //Checar se a senha já foi aprovada
            atendimento.EtapaPreProcedimento.StatusAgendamento = "true";
            var listaSenhas = new ListaSenhas();
            var pathOfSenhasAutorizadas = _fileDataOfSenhaExtractorService.PathToFile;
            if( pathOfSenhasAutorizadas != null)
            {
                listaSenhas = await _fileDataOfSenhaExtractorService.LoadDescritivosFromFile(pathOfSenhasAutorizadas);
            }
            if( listaSenhas != null)
            {
                atendimento.EtapaPreProcedimento.UltimaAtualizacaoSenhas = listaSenhas.UltimaAtualizacao;
                if(listaSenhas.Senhas != null)
                {
                    foreach( var s in listaSenhas.Senhas)
                    {
                        s.NomePaciente.Equals(paciente.Nome);
                        if(s.NomePaciente.Trim().Equals(paciente.Nome?.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Paciente presente na lista de senhas autorizadas");
                            foreach( var p in s.Procedimento)
                            {
                                if(atendimento.EtapaPreProcedimento.Procedimentos.Contains(p))
                                {
                                    atendimento.EtapaPreProcedimento.StatusSenha = "True";
                                    Console.WriteLine("Encontradas senhas de procedimentos aprovadas");
                                    atendimento.MensagemParaMedico = "Senha aprovada, efetuar agendamento";
                                }else
                                {
                                    atendimento.EtapaPreProcedimento.StatusSenha = "false";
                                    Console.WriteLine("Aguardar senha");
                                    atendimento.MensagemParaMedico = $"Aguardar aprovação da senha, última atualização da senhas: {listaSenhas.UltimaAtualizacao} ";
                                }
                            }
                        }
                    }
                }else 
                {
                    atendimento.MensagemParaMedico = "Falha ao carregar lista de consulta, nenhuma senha encontrada";
                    return atendimento;
                }
            }else 
            {
                atendimento.MensagemParaMedico = " Falha ao carregar lista de consulta";
                return atendimento;
            }

        }
        // Progressoes a serem confirmadas, mudança de status para pré procedimento
        var listaPendencias = new List<string>()
        {   
            atendimento.EtapaPreProcedimento.StatusEncaminhamento,
            atendimento.EtapaPreProcedimento.StatusExames, 
            atendimento.EtapaPreProcedimento.StatusInstrumentadora, 
            atendimento.EtapaPreProcedimento.StatusTermoCirurgico,
            atendimento.EtapaPreProcedimento.StatusSenha,
            atendimento.EtapaPreProcedimento.StatusAgendamento
        };
        foreach(var p in listaPendencias)
        {
            var status = p;
            if(status == "false")
            {
                atendimento.MensagemParaMedico = $"O pré procedimento da paciente {paciente.Nome} está pendente, Necessário algumas confirmações";
                return atendimento;
            }       
        }
        atendimento.EtapaPreProcedimento.StatusGeral = "PreOP concluido";
        atendimento.EtapaAtualAtendimento = "Procedimento";
        atendimento.MensagemParaMedico = $"O pré procedimento da paciente {paciente.Nome} está completo";
        return atendimento;
    }
    public Atendimento ValidacaoEtapaProcedimento(Atendimento atendimento, List<Agendamento> agendamentosOfPaciente, Paciente paciente)
    {
        var statusEtapa = new ProcedimentoEtapaStatus();
        var agendamento = new Agendamento(){Data = DateOnly.MinValue};
        if(agendamentosOfPaciente != null && agendamentosOfPaciente.Any())
        {
           foreach (var a in agendamentosOfPaciente)
            {
                // 1. Verificar o status do agendamento/Se o procedimento foi realizado
                if (a.Status == Agendamento.StatusAgendamento.ProcedimentoConcluido)
                {
                    agendamento = a.Data > agendamento.Data ? a : agendamento;// será o ultimo agendamento realizado
                }
            }
        }
        if (agendamento.Status == Agendamento.StatusAgendamento.ProcedimentoConcluido)
        {
            statusEtapa.StatusProcedimento = "Concluído";

            // 2. Se o procedimento foi concluído, verificar outros dados
            // Verifica o status das Instruções
            if (agendamento.Local.Equals("0", StringComparison.OrdinalIgnoreCase)) // Assumindo um campo no Agendamento ou no Procedimento
            {
                statusEtapa.StatusInstrucoes = "false";
            }
            else
            {
                statusEtapa.StatusInstrucoes = "true";
            }
            // Verifica o status do Atestado
            if (agendamento.Local.Equals("0", StringComparison.OrdinalIgnoreCase)) // Assumindo um campo no Agendamento ou no Procedimento
            {
                statusEtapa.StatusAtestado = "false";
            }
            else
            {
                statusEtapa.StatusAtestado = "true";
            }

            // Verifica a Data da Consulta Pós-Operatória
            if (agendamento.Local.Equals("0", StringComparison.OrdinalIgnoreCase)) // Usa .HasValue para DateOnly?
            {
                // statusEtapa.dataConsultaPosOp = DateOnly.MinValue;//não ex
                statusEtapa.StatusConsulta = "false";
            }else
            {
                statusEtapa.dataConsultaPosOp = agendamento.Data; // Pega o valor
                statusEtapa.StatusConsulta = "true";

            }
            // Não atribua se for null, ela já será null por padrão em ProcedimentoEtapaStatus se for DateOnly?
            // Se não for nullable, ela manterá o 0001-01-01
        }
        else
        {
            // 3. Se o procedimento NÃO foi concluído, definir tudo como pendente/não aplicável
            statusEtapa.StatusProcedimento = agendamento.Status.ToString(); // Ou "Pendente / Não Concluído"
            statusEtapa.StatusInstrucoes = "false";
            statusEtapa.StatusAtestado = "false";
            statusEtapa.StatusConsulta = "false";
            statusEtapa.dataConsultaPosOp = DateOnly.MinValue; // Garante que seja null se não for concluído
        }

        // 4. Definir o Status Geral da Etapa (Lógica de Resumo) ESTOU AQUI""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""
        if (statusEtapa.StatusProcedimento == "true" &&
            statusEtapa.StatusInstrucoes == "true" &&
            statusEtapa.StatusAtestado == "true" &&
            statusEtapa.StatusAtestado == "true") // Se a data é obrigatória para "Geral Concluído"
        {
            statusEtapa.StatusGeral = "Procedimento Concluído";
            //$"Etapa do procedimento da paciente {paciente.Nome} concluída";
        }
        else if (statusEtapa.StatusProcedimento == "true") // Se o procedimento foi concluído, mas falta algo
        {
            statusEtapa.StatusGeral = "Procedimento pendente";
            atendimento.MensagemParaMedico = $"Existe alguma pendência a ser resolvida para concluir a etapa do procedimento da paciente {paciente.Nome}";
        }
        else
        {
            if(atendimento.EtapaPreProcedimento.DataAgendamento <= DateOnly.FromDateTime(DateTime.Today)) 
            {
                //Checar cada status de agendamento
                statusEtapa.StatusGeral = "Procedimento pendente"; 
                atendimento.MensagemParaMedico = $"O procedimento da paciente {paciente.Nome} ainda não foi realizado";
            }else
            {
                statusEtapa.StatusGeral = "Procedimento pendente";
                atendimento.MensagemParaMedico = $"O procedimento da paciente {paciente.Nome} passou da data";
            }
        }
     

        atendimento.EtapaProcedimento = statusEtapa;
        return atendimento;
    }
    
    public Atendimento ValidacaoEtapaPosProcedimento(Atendimento atendimento, Agendamento agendamento, Paciente paciente, Prontuario prontuario)
    {
        var statusEtapa = new PosProcedimentoEtapaStatus();

        
        return atendimento;
    }
// Métodos privados
    private static string GetEnumDisplayName<T>(T enumValue) where T : Enum
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        var attribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));
        return attribute?.Name ?? enumValue.ToString();
    }

    public async Task<Stream> GeneratePatientPdfFullReport(string pacienteId)
    {
        // Lógica de negócio para gerar o report
        var paciente = await _pacienteRepository.GetByIdAsync(pacienteId);
         if(paciente == null)
        {
            throw new InvalidOperationException($"Paciente com ID '{pacienteId}' não encontrado.");
        }

        var prontuarios = await _prontuarioRepository.GetProntuariosOfPacienteAsync(pacienteId);
        var agendamentos = await _agendamentoRepository.GetByPacienteIdAsync(pacienteId);

        // Crie e popule seu objeto ReportData aqui
    
        return _pdfGeneratorService.GeneratePatientReportPdf( paciente, prontuarios, agendamentos);
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
    public DateTime ParseDateTime(string datecolumn)
    {
        var datacoluna = datecolumn;
        DateTime data = new DateTime();
        if (!string.IsNullOrWhiteSpace(datacoluna))
        {
            DateTime.TryParse(datacoluna, out data);
        }
        return data;
    }
    private List<CDStatus> ParseCdPendente(string value)
    {
        var list = new List<CDStatus>();

        if (string.IsNullOrWhiteSpace(value))
            return list;

        // Split por "-"
        var items = value.Split('-', StringSplitOptions.RemoveEmptyEntries);

        foreach (var item in items)
        {
            // Cada item deve ter: Descricao,Boolean
            var partes = item.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length != 2)
                continue;

            var descricao = partes[0].Trim();
            var pendenteStr = partes[1].Trim();

            bool pendente = false;
            bool.TryParse(pendenteStr, out pendente);

            list.Add(new CDStatus
            {
                Descricao = descricao,
                Pendente = pendente
            });
        }

        return list;
    }
}