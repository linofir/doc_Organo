using System.ComponentModel.DataAnnotations;
using DocAPI.Core.Models;
using DocAPI.Core.Repositories;
using DocAPI.Services;
using NPOI.SS.Formula.Functions;
// using DocAPI.Data.Dtos.Atendimento;   

namespace DocAPI.Infrastructure.SheetsDb;
public class AtendimentoSheetsRepository : IAtendimentoRepository
{

    private readonly IPacienteRepository _pacienteRepository;
    private readonly IProntuarioRepository _prontuarioRepository;
    private readonly IAgendamentoRepository _agendamentoRepository; // Se precisar de agendamento
    private readonly PdfGeneratorService _pdfGeneratorService;

    public AtendimentoSheetsRepository(IPacienteRepository pacienteRepository,
                         IProntuarioRepository prontuarioRepository,
                         IAgendamentoRepository agendamentoRepository,
                         PdfGeneratorService pdfGeneratorService)
    {
        _pacienteRepository = pacienteRepository;
        _prontuarioRepository = prontuarioRepository;
        _agendamentoRepository = agendamentoRepository;
        _pdfGeneratorService = pdfGeneratorService;
    }

    public async Task<Stream> CreateReportByIdAsync( string pacienteId )
    {
        return await GeneratePatientPdfFullReport(pacienteId);
    }
    public async Task<Atendimento> CreateReportFollwUpByIdAsync( string pacienteId )
    {
        return await InstantiateAtendimento(pacienteId);
        // throw new NotImplementedException();
    }
    public async Task<Atendimento> InstantiateAtendimento ( string pacienteId )
    {
        // Criar lógicas para definirem a etapa do atendimento, inicializando.
        var atendimento = new Atendimento()
        {
            EtapaConsulta = new ConsultaEtapaStatus()
        };
        if (string.IsNullOrEmpty(atendimento.ID))
        {
            atendimento.ID = Guid.NewGuid().ToString();
        }
        // Para Etapa COnsulta, validando cadastro
        var paciente = await _pacienteRepository.GetByIdAsync(pacienteId);
        if(paciente == null)
        {
            // throw new InvalidOperationException($"Paciente com ID '{pacienteId}' não encontrado.");
            atendimento.EtapaConsulta.CadastroConfirmado = false;
            atendimento.EtapaAtualAtendimento = "Atendimento Pendente";
            atendimento.MensagemParaMedico = $"Problema ao identificar o cadastro da paciente com ID: {pacienteId}";
            return atendimento;
        }
        // Cadastro de paciente confirmado
        atendimento.PacienteId = pacienteId;
        atendimento.EtapaConsulta.CadastroConfirmado = true;
        atendimento.EtapaConsulta.StatusGeral = "Consulta Pendente";
        atendimento.EtapaAtualAtendimento = "Inicialização";

        // Validando Consulta ainda lidando somente com um prontuario padrão(que inicia um atendimento)
        var prontuariosOfPaciente = await _prontuarioRepository.GetProntuariosOfPacienteAsync(paciente);
        // Console.WriteLine($"quantidade de prontuários encontrados no atendimento: {prontuariosOfPaciente.Count()}");

        var allAcoesCD = Enum.GetValues(typeof(AcoesCD))
                             .Cast<AcoesCD>()
                             .Select(action => new
                             {
                                 Value = action,
                                 DisplayName = GetEnumDisplayName(action)
                             })
                             .ToList();
       
        var prontuariosId = new List<string>(){};

        
        // Validar se consulta foi realizada Data, Cds,  para prontuário existente(validar tipos de prontuarios).
        if(prontuariosOfPaciente != null && prontuariosOfPaciente.Any())
        {
           foreach (var p in prontuariosOfPaciente)
            {
                prontuariosId.Add(p.ID);
            }
            atendimento.ProntuarioId = prontuariosId;
            var prontuarioInicialTeste = prontuariosOfPaciente[0];
            atendimento.EtapaConsulta.DataConsultaConcluida = prontuarioInicialTeste.DataConsulta;
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
                // Ignorar "Sem Info" pois não é uma ação a ser "preenchida", mas sim um status de não-preenchimento
                // Você pode ajustar essa lógica se "Sem Info" tiver outro significado.
                // if (action.Value == AcoesCD.SemInformacao)
                // {
                //     atendimento.MensagemParaMedico = $"Atendimento aberto da paciente {paciente.Nome}, mas sem as informações de CD.";
                //     continue;
                //     // continue; 
                // }
                if (prontuarioInicialTeste.CD != null && prontuarioInicialTeste.CD.Contains(AcoesCD.SemInformacao) && prontuarioInicialTeste.CD.Count == 1)
                {
                    // Se "Sem Info" é a única CD, então todas as outras são pendentes.
                    // Populate cdStatusList marcando tudo como Pendente, exceto SemInformacao.
                    // ... (implementar essa lógica)
                    atendimento.MensagemParaMedico = $"Atendimento aberto da paciente {paciente.Nome}, com registro de 'Sem Info' nas CDs.";
                    // Pode ser um bom ponto para retornar ou definir um status geral específico.
                }

                // Verificar se a ação está presente na lista CD do prontuário
                var isPresent = prontuarioInicialTeste.CD?.Contains(action.Value) ?? false;

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
        

        //Para Etapa Pré Procedimento
        //Checar senhas autorizadas, implementar lógica para verificação das senhas
        
        var agendamentosOfPaciente = await _agendamentoRepository.GetByPacienteIdAsync(pacienteId);
        var agendamentosId = new List<string>(){};
        if(agendamentosOfPaciente != null && agendamentosOfPaciente.Any())
        {
           foreach (var a in agendamentosOfPaciente)
            {
                agendamentosId.Add(a.ID);
            }

        }
        // Progressoes a serem confirmadas, mudança de status para cada CD

        // Procedimento
        // Confimar Execução de procedimentos, status.
        // Atestado.

        // Pós Procedimento
        // Data Consulta pós OP
        // Geração de novo prontuário e CDs
        // Alarme para Seguimento médico.

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