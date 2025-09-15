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
       throw new NotImplementedException();
    }
    public Task<Atendimento?> GetByIdAsync(string id)
    {
       throw new NotImplementedException();
    }
    public Task CreateAsync(Atendimento novoAtendimento)
    {
       throw new NotImplementedException();
    }
    public Task UpdateAsync(Atendimento atendimento, string id)
    {
       throw new NotImplementedException();
    }
    public Task DeleteAsync(string id)
    {
       throw new NotImplementedException();
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
    
    public async Task AddAtendimentooAsync(Atendimento atendimento)
    {
        // Definir quais são os dados a serem persisitidos no DB, desconsiderando os já existentes em outras planilhas.
        Console.WriteLine($"Comunicando com DB..." );
        var agendamentoSheets = await _sheetsDB.LerRangeAsync("Atendimento!A3:Q");//definr tabela
        int novaLinhaIndex = agendamentoSheets.Count(r => r.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString()))) + 3;
        //Como é melhor armazenar o status, é preciso validar e preparar os dados antes de armazena-los
        if (string.IsNullOrEmpty(atendimento.ID))
        {
            atendimento.ID = Guid.NewGuid().ToString();
        };


        // ValueRange body = CreateAgendamentoToSheets(agendamento); Definir método
        
        // 3. Escrever os dados na próxima linha disponível
        string rangeDestino = $"Agendamentos!A{novaLinhaIndex}:Q{novaLinhaIndex}";
        Console.WriteLine($"O novo Agendamento será acrescentado na { rangeDestino}");
        // await _sheetsDB.WriteRangeAsync(rangeDestino, body.Values);
    }

    public async Task<List<Atendimento>> GetAtendimentosAsync()
    {
        var values = await _sheetsDB.LerRangeAsync("Atendimento!A3:H"); // de A até a coluna ID
        var allAtendimentos = new List<Atendimento>();
        var limit = values.Count;
        //Console.WriteLine($"Total de linhas com algum dado: {limit}");       
        for (int i = 0; i < limit; i++)
        {
            var row = values[i];
            if (row.All(cell => string.IsNullOrWhiteSpace(cell?.ToString()))) continue;
            //Confere se tem alguma coluna vazia
            if (row.Count < 7)
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
            EtapaConsulta = new ConsultaEtapaStatus(),
            EtapaPreProcedimento = new PreProcedimentoEtapaStatus(),
            EtapaProcedimento = new ProcedimentoEtapaStatus(),
            EtapaPosProcedimento = new PosProcedimentoEtapaStatus()
        };
        var pacienteId = row[1].ToString();

        // dados a serem validados para cada paciente
        if(!string.IsNullOrWhiteSpace(pacienteId))
        {
            atendimento = await AtualizarAtendimento(pacienteId, atendimento);
        }
        return atendimento;
    }
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
            atendimento.EtapaConsulta!.CadastroConfirmado = false;
            atendimento.MensagemParaMedico = $"Problema ao identificar o cadastro da paciente com ID: {atendimento.PacienteId}";
            return atendimento;
        }
        // Cadastro de paciente confirmado
        // atendimento.PacienteId = pacienteId;
        atendimento.EtapaConsulta!.CadastroConfirmado = true;
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
        var prontuariosOfPaciente = await _prontuarioRepository.GetProntuariosOfPacienteAsync(paciente);
        // var procedimentos = new List<string>();
        
        // Console.WriteLine($"quantidade de prontuários encontrados no atendimento: {prontuariosOfPaciente.Count()}");
        if(prontuariosOfPaciente != null && prontuariosOfPaciente.Any())
        {
            // Delimitando por prontuarios do Tipo Solicitacao(que inicia um atendimento), coleta o ultimo prontuário
            var prontuarioSolicitacao = new Prontuario(){DataConsulta = DateOnly.MinValue};
            foreach (var p in prontuariosOfPaciente)
            {
                if(p.Tipo == "Solicitacao")
                {
                    prontuariosId.Add(p.ID);
                    prontuarioSolicitacao = p.DataConsulta > prontuarioSolicitacao.DataConsulta ? p : prontuarioSolicitacao;
                }
            }
            atendimento.ProntuariosId = prontuariosId;
            atendimento.EtapaConsulta.DataConsultaConcluida = prontuarioSolicitacao.DataConsulta;
            atendimento.EtapaPreProcedimento.Procedimentos = prontuarioSolicitacao.SolicitacaoInternacao.Procedimentos;
            // Console.WriteLine($"Valor da data : {atendimento.EtapaConsulta.DataConsultaConcluida}");
            atendimento.EtapaAtualAtendimento = "Consulta";
            
            if(atendimento.EtapaConsulta.DataConsultaConcluida <= DateOnly.FromDateTime(DateTime.Today)) 
            {
                atendimento.EtapaConsulta.ConsultaConcluida = true;
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
        atendimento.EtapaPreProcedimento.StatusEncaminhamento = "true";
        atendimento.EtapaPreProcedimento.StatusExames = "true";
        atendimento.EtapaPreProcedimento.StatusInstrumentadora = "true";
        atendimento.EtapaPreProcedimento.StatusTermoCirurgico = "true";
        var agendamentosId = new List<string>(){};
        var agendamentoTeste = new Agendamento(){Data = DateOnly.MinValue};
        if(agendamentosOfPaciente != null && agendamentosOfPaciente.Any())
        {
           foreach (var a in agendamentosOfPaciente)
            {
                agendamentosId.Add(a.ID);
                agendamentoTeste = a.Data > agendamentoTeste.Data ? a : agendamentoTeste;// será o ultimo agendamento realizado, definir melhor
            }
            atendimento.AgendamentosId = agendamentosId;
            // agendamentoTeste = agendamentosOfPaciente[0];
            atendimento.EtapaPreProcedimento.DataAgendamento = agendamentoTeste.Data;
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

        var prontuarios = await _prontuarioRepository.GetProntuariosOfPacienteAsync(paciente);
        var agendamentos = await _agendamentoRepository.GetByPacienteIdAsync(pacienteId);

        // Crie e popule seu objeto ReportData aqui
    
        return _pdfGeneratorService.GeneratePatientReportPdf( paciente, prontuarios, agendamentos);
    }
    
}