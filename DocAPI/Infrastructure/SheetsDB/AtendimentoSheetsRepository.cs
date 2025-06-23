using DocAPI.Core.Repositories;
using DocAPI.Services;
using DocAPI.Data.Dtos.Relatorio;   
public class RelatorioRepository : IAtendimentoRepository
{

    private readonly IPacienteRepository _pacienteRepository;
    private readonly IProntuarioRepository _prontuarioRepository;
    private readonly IAgendamentoRepository _agendamentoRepository; // Se precisar de agendamento
    private readonly PdfGeneratorService _pdfGeneratorService;

    public RelatorioRepository(IPacienteRepository pacienteRepository,
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
        return GeneratePatientPdfFullReport(string pacienteId)
    
    }

    public async Task<Stream> InstantiateRelatorioDto   ( string pacienteId )
    {
        var paciente = await _pacienteRepository.GetByIdAsync(pacienteId);
        if(paciente == null)
        {
            throw new InvalidOperationException($"Paciente com ID '{pacienteId}' não encontrado.");
        }
        var prontuariosOfPaciente = await _prontuarioRepository.GetProntuariosOfPacienteAsync(paciente);
        var agendamentos = await _agendamentoRepository.GetByPacienteIdAsync(pacienteId);

        // Criar lógicas para definirem a etapa do atendimento

        // Para Etapa COnsulta
        // Cadastro de paciente confirmado
        // Consulta concluida, prontuário existente, fonecer Data
        // Fornecer CDs geradas na consulta e status de cada CD

        //Para Etapa Pré Procedimento
        // Progressoes a serem confirmadas, mudança de status para cada CD

        // Procedimento
        // Confimar Execução de procedimentos, status.
        // Atestado.

        // Pós Procedimento
        // Data Consulta pós OP
        // Geração de novo prontuário e CDs
        // Alarme para Seguimento médico.

        return _pdfGeneratorService.GeneratePatientReportPdf( paciente, prontuariosOfPaciente);
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
    
        return _pdfGeneratorService.GeneratePatientReportPdf( paciente, prontuarios);
    }
}