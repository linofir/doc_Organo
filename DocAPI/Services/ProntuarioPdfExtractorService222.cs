using Tabula;
using Tabula.Extractors;
using UglyToad.PdfPig;
using UglyToad.PdfPig.AcroForms;
using UglyToad.PdfPig.AcroForms.Fields;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Writer;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using DocAPI.Core.Models;

public class ProntuarioPdfExtractorService2
{
    //pathPDF  @"C:\Users\lino\Dropbox\io\Doc_Organo\teste_prontuario.pdf";
    private Paciente Paciente {get ; set ;}
    
    public ProntuarioPdfExtractorService2(Paciente paciente)
    {
        Paciente = paciente;
    }

    public async Task<Prontuario> ExtrairProntuarioDePdfAsync( string pathPdf)
    {
        var pageNumber = -1;
        using (var documentTest = PdfDocument.Open(pathPdf))
        {
            var builder = new PdfDocumentBuilder { };
            var boundingBlocks = new List<PdfRectangle>();
            PdfDocumentBuilder.AddedFont font = builder.AddStandard14Font(Standard14Font.Helvetica);
            var pageBuilder = builder.AddPage(documentTest, pageNumber);
            pageBuilder.SetStrokeColor(0, 255, 0);
            var page = documentTest.GetPage(pageNumber);

            // 1. Check if has form fields
            bool hasForm = documentTest.TryGetForm(out AcroForm form);
            Console.WriteLine($"Existe um form? {hasForm}");

            // 2. Get fields for page
            var fieldsForPage = form.GetFieldsForPage(pageNumber);
        }

        using var document = PdfDocument.Open(pathPdf);
        var textoCompleto = new StringBuilder();

        foreach (var page in document.GetPages())
        {
            textoCompleto.AppendLine(page.Text);
        }

        var texto = textoCompleto.ToString();

        var prontuario = new Prontuario
        {
            DescricaoBasica = ExtrairDescricaoBasica(texto),
            AGO = ExtrairAGO(texto),
            Antecedentes = ExtrairAntecedentes(texto),
            AntecedentesFamiliares = ExtrairAF(texto),
            CD = ExtrairAcoesCD(texto),
            InformacoesExtras = "", // pode ser extraído ou preenchido depois
            Exames = ExtrairExames(texto),
            SolicitacaoInternacao = ExtrairInternacao(texto),
            DataRequisicao = DateTime.Now // ou extraído, se presente
        };

        return prontuario;
    }

    private DescricaoBasica ExtrairDescricaoBasica(string texto)
    {
        var nome = ExtrairCampo(texto, "Nome da paciente", "Idade");
        var idadeStr = ExtrairCampo(texto, "Idade", "Profissão");
        var idade = int.TryParse(idadeStr, out var i) ? i : 0;

        return new DescricaoBasica
        {
            NomePaciente = Paciente.Nome,
            Idade = Paciente.Idade,
            Profissao = ExtrairCampo(texto, "Profissão", "Religião"),
            Religiao = ExtrairCampo(texto, "Religião", "QD"),
            QD = ExtrairCampo(texto, "QD", "CRM"),
            AtividadeFisica = "completar PDF", // se existir
            PacienteId = Paciente.ID, // preencher no consumo do serviço
        };
    }

    private AGO ExtrairAGO(string texto)
    {
        return new AGO
        {
            Menarca = ExtrairCampo(texto, "Menarca", "DUM"),
            DUM = ExtrairCampo(texto, "DUM", "Paridade"),
            Paridade = ExtrairCampo(texto, "Paridade", "Desejo de Gestação"),
            DesejoGestacao = ExtrairMarcado(texto, "Desejo de Gestação"),
            VacinaHPV = ExtrairVacinaHPV(texto),
            CCO = ExtrairCampo(texto, "CCO", "MAC/TRH"),
            MAC_TRH = ExtrairCampo(texto, "MAC/TRH", "Comorbidades"),
        };
    }

    private Antecedentes ExtrairAntecedentes(string texto)
    {
        return new Antecedentes
        {
            Comorbidades = ExtrairCampo(texto, "Comorbidades", "Medicação"),
            Medicacao = ExtrairCampo(texto, "Medicação", "Cirurgias"),
            Cirurgias = ExtrairCampo(texto, "Cirurgias", "Alergias"),
            Alergias = ExtrairCampo(texto, "Alergias", "Vícios"),
            Vicios = ExtrairCampo(texto, "Vícios", "Hábito intestinal"),
            HabitoIntestinal = ExtrairCampo(texto, "Hábito intestinal", "Vacinas"),
            Vacinas = ExtrairCampo(texto, "Vacinas", "Dra")
        };
    }

    private AntecedentesFamiliares ExtrairAF(string texto)
    {
        return new AntecedentesFamiliares
        {
            Neoplasias = ExtrairCampo(texto, "Neoplasias", "Dra"),
            Comorbidades = "Completar o PDF" // não está claro no modelo, pode ser adicionado
        };
    }

    private List<AcoesCD> ExtrairAcoesCD(string texto)
    {
        var acoes = new List<AcoesCD>();

        if (texto.Contains("(X) Pedido de exames")) acoes.Add(AcoesCD.PedidoExame);
        if (texto.Contains("(X) Pedido de Internação")) acoes.Add(AcoesCD.PedidoInternacao);
        if (texto.Contains("(X) Indicação de encaminhamentos")) acoes.Add(AcoesCD.IndicacaoEncaminhamentos);
        if (texto.Contains("(X) Informativo de Instrumentadora")) acoes.Add(AcoesCD.InformativosInstrumentadora);
        if (texto.Contains("(X) Entrega de pasta")) acoes.Add(AcoesCD.PastaInformativa);

        return acoes;
    }

    private List<Exame> ExtrairExames(string texto)
    {
        var exames = new List<Exame>();
        var regex = new Regex(@"40\d{5}\s([A-Z0-9 -]+)\s\(\d+\)\d+", RegexOptions.Multiline);

        foreach (Match match in regex.Matches(texto))
        {
            var partes = match.Value.Split(' ', 2);
            if (partes.Length == 2)
            {
                exames.Add(new Exame
                {
                    Codigo = partes[0],
                    Nome = LimparTexto(partes[1])
                });
            }
        }

        return exames;
    }

    private Internacao ExtrairInternacao(string texto)
    {
        return new Internacao
        {
            Procedimentos = new List<string> {
                ExtrairCampo(texto, "Descrição", "Qtde.")
            },
            IndicacaoClinica = ExtrairCampo(texto, "Indicação Clínica", "CID"),
            CID = ExtrairCampo(texto, "CID", "Data da Solicitação"),
            TempoDoenca = ExtrairCampo(texto, "Tempo da doença", "Diárias"),
            Diarias = ExtrairCampo(texto, "Diárias", "Internação"),
            Tipo = ExtrairCampo(texto, "Tipo de Internação", "Regime"),
            Regime = ExtrairCampo(texto, "Regime de Internação", "Caráter"),
            Carater = ExtrairCampo(texto, "Caráter de Internação", "OPME"),
            UsaOPME = texto.Contains("Previsão de uso OPME") && texto.Contains("S"),
            Local = ExtrairCampo(texto, "Hospital / local Solicitado", "Data da Solicitação"),
            Guia = long.TryParse(ExtrairCampo(texto, "GUIA DE SOLICITAÇÃO", "Registro ANS"), out var guia) ? guia : 0
        };
    }

    // Métodos auxiliares
    private string ExtrairCampo(string texto, string inicio, string proximoCampo)
    {
        var padrao = $@"{inicio}\s+(.*?)\s+{proximoCampo}";
        var match = Regex.Match(texto, padrao, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        return match.Success ? LimparTexto(match.Groups[1].Value) : "";
    }

    private string ExtrairMarcado(string texto, string campo)
    {
        if (texto.Contains($"{campo}\n(X) Sim")) return "Sim";
        if (texto.Contains($"{campo}\n(X) Não")) return "Não";
        return "Sem informação";
    }

    private StatusVacinaHPV ExtrairVacinaHPV(string texto)
    {
        if (texto.Contains("3 Doses")) return StatusVacinaHPV.TresDoses;
        if (texto.Contains("2 Doses")) return StatusVacinaHPV.DuasDoses;
        if (texto.Contains("1 Dose")) return StatusVacinaHPV.UmaDose;
        if (texto.Contains("Sem vacina")) return StatusVacinaHPV.SemVacina;
        return StatusVacinaHPV.SemInfo;
    }

    private string LimparTexto(string input)
    {
        return Regex.Replace(input ?? "", @"\s{2,}", " ").Trim();
    }
}
