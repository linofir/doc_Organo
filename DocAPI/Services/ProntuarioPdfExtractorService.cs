using Tabula;
using Tabula.Extractors;
using UglyToad.PdfPig;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using DocAPI.Core.Models;
using UglyToad.PdfPig.Graphics.Colors;
using UglyToad.PdfPig.AcroForms;
using UglyToad.PdfPig.AcroForms.Fields;
public class ProntuarioPdfExtractorService
{
    //pathPDF  @"C:\Users\lino\Dropbox\io\Doc_Organo\teste_prontuario.pdf";
    private Paciente Paciente {get ; set ;}
    public ProntuarioPdfExtractorService(Paciente paciente)
    {
        Paciente = paciente;
    }

    public async Task<Prontuario> ExtrairProntuarioDePdfAsync( string pathPdf)
    {
        using var document = PdfDocument.Open(pathPdf);

        //1. Verifica se possui AcroForm
        bool hasForm = document.TryGetForm(out AcroForm form);
        if (hasForm == true) Console.WriteLine("📋 PDF contém formulário (AcroForm). Usando extração via campos.");


        // if (document.TryGetForm(out var form) && form.Fields.Any())
        // {
        //     foreach (var field in form.GetFields())
        //     {
        //             var nomeDoCampo = field.ToString(); // Nome "descritivo", útil para debug
        //             var valor = field.GetFieldValue();
        //             var tipo = field.FieldType;

        //         Console.WriteLine($"Campo: {nomeDoCampo} | Tipo: {tipo} | Valor: {valor}");
        //     }
        //     Console.WriteLine("📋 PDF contém formulário (AcroForm). Usando extração via campos.");
        //     // return ExtrairProntuarioViaAcroForm(form);
        //     return new Prontuario{};
        // }
        
        Console.WriteLine("🔍 PDF não contém formulário. Usando extração via texto/regex.");

        // fallback para regex

        var textoCompleto = new StringBuilder();
        var paginasTexto = new List<string>();

        foreach (var page in document.GetPages())   
        {
            paginasTexto.Add(page.Text);
            //textoCompleto.AppendLine(page.Text);
        }

        //var texto = textoCompleto.ToString();
        var prontuario = ExtrairProntuarioViaRegex(paginasTexto);
        // Console.WriteLine($"página 7: {paginasTexto[6]}");
        // Console.WriteLine($"página 8: {paginasTexto[7]}");
        Console.WriteLine($"página 9: {paginasTexto[8]}");

        return prontuario;
    }
    private Prontuario ExtrairProntuarioViaRegex(List<string> paginasTexto)
    {
        return  new Prontuario
        {
            DescricaoBasica = ExtrairDescricaoBasica(paginasTexto),
            AGO = ExtrairAGO(paginasTexto),
            Antecedentes = ExtrairAntecedentes(paginasTexto),
            AntecedentesFamiliares = ExtrairAF(paginasTexto),
            CD = ExtrairAcoesCD(paginasTexto),
            // InformacoesExtras = "", // pode ser extraído ou preenchido depois
            Exames = ExtrairExames(paginasTexto),
            // SolicitacaoInternacao = ExtrairInternacao(paginasTexto),
            // DataRequisicao = DateTime.Now // ou extraído, se presente
        };

    }

    private DescricaoBasica ExtrairDescricaoBasica(List<string> paginasTexto)
    {
        for(int i = 0; i == paginasTexto.Count(); i++)
        {
            if (paginasTexto[i].Contains("Descrição básica")) Console.WriteLine($"página {i+1}: {paginasTexto[i]}");
        }
        var listaConsdicoesFim = new List<string>(){"*Nome da Paciente:","*Idade:","*Profissão:","*Religião:","*QD(Queixa/Encaminhamento):","Dra Isis Caroline Firmano"};
        if (paginasTexto.Count == 0) return new DescricaoBasica();
        if (paginasTexto[5].Contains("Descrição básica") )
        {
            return new DescricaoBasica
            {
                NomePaciente = Paciente.Nome,
                Idade = Paciente.Idade,
                Profissao = ExtrairCampoCondicional(paginasTexto[5], listaConsdicoesFim[2], listaConsdicoesFim),
                Religiao = ExtrairCampoCondicional(paginasTexto[5], listaConsdicoesFim[3], listaConsdicoesFim),
                QD = ExtrairCampoCondicional(paginasTexto[5], listaConsdicoesFim[4], listaConsdicoesFim),
                AtividadeFisica = ExtrairCampoCondicional(paginasTexto[5], listaConsdicoesFim[5], listaConsdicoesFim), // se existir
                PacienteId = Paciente.ID, // preencher no consumo do serviço
            };
        }else return new DescricaoBasica();

    }

    private AGO ExtrairAGO(List<string> paginasTexto)
    {
        var listaConsdicoesFim = new List<string>(){"*DUM:","*Paridade:","*Desejo de Gestação:","*Vacina HPV:","*Intercorrências:","*Amamentação:","*Vida Sexual:","*Relacionamento:","*Parceiros/as:","*Coitarca:","*Menarca:","*IST:","*MAC_TRH:","Dra Isis Caroline Firmano"};
        var listaConsdicoesFimSegunda = new List<string>(){"*CCO:", "Dra Isis Caroline Firmano"};
        if (paginasTexto.Count == 0) return new AGO();
        if (paginasTexto[1].Contains("AGO(Antecedentes Ginecológicos Obstétricos)") && paginasTexto[2].Contains("AGO/Continua(Antecedentes Ginecológicos Obstétricos)") )
        {
            // Console.WriteLine("página válida para AGO");
            return new AGO
            {
                DUM = ExtrairCampoCondicional(paginasTexto[1], "*DUM:", listaConsdicoesFim),
                Paridade = ExtrairCampoCondicional(paginasTexto[1], "*Paridade:", listaConsdicoesFim),
                DesejoGestacao = ExtrairMarcado(ExtrairCampoCondicional(paginasTexto[1], "*Desejo de Gestação:", listaConsdicoesFim)),
                VacinaHPV = ExtrairVacinaHPV(ExtrairCampoCondicional(paginasTexto[1], "*Vacina HPV:", listaConsdicoesFim)),//identificar o ENUM texto
                Intercorrencias = ExtrairCampoCondicional(paginasTexto[1], "*Intercorrências:", listaConsdicoesFim),
                Amamentacao = ExtrairCampoCondicional(paginasTexto[1], "*Amamentação:", listaConsdicoesFim),
                VidaSexual = ExtrairCampoCondicional(paginasTexto[1], "*Vida Sexual:", listaConsdicoesFim),
                Relacionamento = ExtrairCampoCondicional(paginasTexto[1], "*Relacionamento:", listaConsdicoesFim),
                Parceiros = ExtrairCampoCondicional(paginasTexto[1], "*Parceiros/as:", listaConsdicoesFim),
                Coitarca = ExtrairCampoCondicional(paginasTexto[1], "*Coitarca:", listaConsdicoesFim),
                Menarca = ExtrairCampoCondicional(paginasTexto[1], "*Menarca:", listaConsdicoesFim),
                IST = ExtrairCampoCondicional(paginasTexto[1], "*IST:", listaConsdicoesFim),
                MAC_TRH = ExtrairCampoCondicional(paginasTexto[1], "*MAC_TRH:", listaConsdicoesFim),
                CCO = ExtrairCampoCondicional(paginasTexto[2], "*CCO:", listaConsdicoesFimSegunda),//mudar a identificacao da pávina"AGO/Continua(Antecedentes Ginecológicos Obstétricos)
            };
        }else return new AGO();
    }

    private Antecedentes ExtrairAntecedentes(List<string> paginasTexto)
    {
        var listaConsdicoesFim = new List<string>(){"*Comorbidades:","*Medicação em uso:","*Neoplasias:","*Cirurgias:","*Alergias:","*Vícios:","*Hábito intestinal:","*Vacinas:","Dra Isis Caroline Firmano"};
        if (paginasTexto.Count == 0) return new Antecedentes();
        if (paginasTexto[3].Contains("AP(Antecedentes)"))
        {
            return new Antecedentes
            {
                Comorbidades = ExtrairCampoCondicional(paginasTexto[3], listaConsdicoesFim[0], listaConsdicoesFim),
                Medicacao = ExtrairCampoCondicional(paginasTexto[3], listaConsdicoesFim[1], listaConsdicoesFim),
                Neoplasias = ExtrairCampoCondicional(paginasTexto[3], listaConsdicoesFim[2], listaConsdicoesFim),
                Cirurgias = ExtrairCampoCondicional(paginasTexto[3], listaConsdicoesFim[3], listaConsdicoesFim),
                Alergias = ExtrairCampoCondicional(paginasTexto[3], listaConsdicoesFim[4], listaConsdicoesFim),
                Vicios = ExtrairCampoCondicional(paginasTexto[3], listaConsdicoesFim[5], listaConsdicoesFim),
                HabitoIntestinal = ExtrairCampoCondicional(paginasTexto[3], listaConsdicoesFim[6], listaConsdicoesFim),
                Vacinas = ExtrairCampoCondicional(paginasTexto[3], listaConsdicoesFim[7], listaConsdicoesFim)
            };
        }else return new Antecedentes();
    }

    private AntecedentesFamiliares ExtrairAF(List<string> paginasTexto)
    {
        var listaConsdicoesFim = new List<string>(){"*Neoplasias:","*Comorbidades:","Dra Isis Caroline Firmano"};
        if (paginasTexto.Count == 0) return new AntecedentesFamiliares();
        if (paginasTexto[0].Contains("(Antecedestes Familiares)"))
        {
            // Console.WriteLine("página válida para Antecedestes Familiares");
            return new AntecedentesFamiliares
            {
                Neoplasias = ExtrairCampoCondicional(paginasTexto[0],"*Neoplasias:", listaConsdicoesFim),
                Comorbidades = ExtrairCampoCondicional(paginasTexto[0], "*Comorbidades:", listaConsdicoesFim ) // não está claro no modelo, pode ser adicionado
            };
        }else return new AntecedentesFamiliares();
    }

    private List<AcoesCD> ExtrairAcoesCD(List<string> paginasTexto)
    {
        var acoes = new List<AcoesCD>();

        if (paginasTexto[4].Contains("(X) Pedido de exames")) acoes.Add(AcoesCD.PedidoExame);
        if (paginasTexto[4].Contains("(X) Pedido de Internação")) acoes.Add(AcoesCD.PedidoInternacao);
        if (paginasTexto[4].Contains("(X) Indicação de encaminhamentos")) acoes.Add(AcoesCD.IndicacaoEncaminhamentos);
        if (paginasTexto[4].Contains("(X) Informativo de Instrumentadora")) acoes.Add(AcoesCD.InformativosInstrumentadora);
        if (paginasTexto[4].Contains("(X) Entrega de pasta")) acoes.Add(AcoesCD.PastaInformativa);

        return acoes;
    }

    private List<Exame> ExtrairExames(List<string> paginasTexto)
    {
        // Regex para identificar cada bloco de exame.
        // Procura por:
        // ([14]\d{7})        -> Grupo 1: O Código do exame (8 dígitos, começando com 1 ou 4).
        // (                  -> Início do Grupo 2: O restante do bloco do exame (nome + (quantidade) + número de ordem).
        //    (?:(?!\s*[14]\d{7}|\s*Dra\s+Isis\s+Caroline\s+Firmano|\s*Indicação\s+Clínica:).)*?
        //                   -> Este é um padrão "consuma tudo até o próximo delimitador sem incluí-lo".
        //                      (?: ... )*? -> Grupo não-capturador, zero ou mais vezes, não-ganancioso.
        //                      (?!\s*[14]\d{7} ... ) -> Negative Lookahead: Garante que o que está sendo consumido NÃO seja
        //                                               o início do próximo exame, nem o rodapé, nem "Indicação Clínica:".
        // )
        // (?=                 -> Positive Lookahead: Onde este bloco termina.
        //    \s*[14]\d{7}     -> Início do próximo código de exame.
        //    | \s*Dra\s+Isis\s+Caroline\s+Firmano  -> Fim da seção de exames.
        //    | \s*Indicação\s+Clínica:.* -> Marcador "Indicação Clínica" (para exames de USG).
        //    | $              -> Fim da string/página.
        // )
        var regexBlocoExame = new Regex(
            @"([14]\d{7})( (?:(?!\s*[14]\d{7}|\s*Dra\s+Isis\s+Caroline\s+Firmano|\s*Indicação\s+Clínica:).)*?)" +
            @"(?=\s*[14]\d{7}|\s*Dra\s+Isis\s+Caroline\s+Firmano|\s*Indicação\s+Clínica:.*|$)",
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled
        );
        
        // Regex para extrair detalhes (Código e Nome) de um bloco de exame já isolado.
        // ^                    -> Início do bloco.
        // ([14]\d{7})          -> Código do exame (8 dígitos, começa com 1 ou 4), grupo 1.
        // \s+                  -> Pelo menos um espaço.
        // (                    -> Início do grupo 2: Nome do Exame.
        //    [A-Z0-9\s\-.(),/&_]+? -> Caracteres permitidos, não-ganancioso.
        // )
        // (?:\s*\(\d+\))?      -> Opcional: (1), (2), etc.
        // (\d{1,2})?           -> Opcional: Número de ordem (1 ou 2 dígitos) - Grupo 3, se precisar.
        // (?:\s*Indicação\s+Clínica:.*)? -> Opcional: "Indicação Clínica: ROTINA" (para USG).
        // $                    -> Fim do bloco.
        var regexDetalheExame = new Regex(
            @"^([14]\d{7})\s+([A-Z0-9\s\-.(),/&_]+?)(?:\s*\(\d+\))?(\d{1,2})?(?:\s*Indicação\s+Clínica:.*)?$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled
        );

        var examesEncontrados = new List<Exame>();
        Console.WriteLine($"Total de páginas para verificar: {paginasTexto.Count}");

        for(int i = 0; i < paginasTexto.Count(); i++)
        {
            var textoPagina = paginasTexto[i];
            //Console.WriteLine($"--- Verificando Página {i + 1} ---");
            if(textoPagina.Contains("Pedido de ExameCaráter da solicitação:"))
            {
                // Console.WriteLine($"página de exame {i+1} encontrada: {paginasTexto[i]}");
                var blocosMatches = regexBlocoExame.Matches(textoPagina);
                // Console.WriteLine($"Número de blocos de exame encontrados na Página {i + 1}: {blocosMatches.Count}");
                if (blocosMatches.Count == 0)
                {
                    Console.WriteLine("Nenhum bloco de exame encontrado. Verifique regexBlocoExame.");
                }
                // 2. Extrair cada exame da página identificada
                foreach (Match blocoMatch in blocosMatches)
                {
                    // blocoMatch.Value contém o "bloco" de texto do exame (ex: "40304361 HEMOGRAMA (1)1")
                    // ou "40901300 USG TRANSVAGINAL (1)1Indicação Clínica: ROTINA"
                    var blocoTexto = blocoMatch.Value.Trim();
                    // O primeiro grupo do blocoMatch.Value já é o código, e o segundo é o nome+resto
                    // Mas vamos usar regexDetalheExame para ser mais seguro

                    var detalheMatch = regexDetalheExame.Match(blocoTexto);

                    if (detalheMatch.Success)
                    {
                        var codigoExame = detalheMatch.Groups[1].Value;
                        var nomeExame = LimparTexto(detalheMatch.Groups[2].Value);

                        examesEncontrados.Add(new Exame
                        {
                            Codigo = codigoExame,
                            Nome = nomeExame
                        });
                        // Console.WriteLine($"  -> Exame Extraído: Código='{codigoExame}', Nome='{nomeExame}'");
                    }
                    else
                    {
                        Console.WriteLine($"  -> ERRO: Não foi possível detalhar o bloco: '{blocoTexto}'");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Página {i + 1} não contém o marcador 'Pedido de ExameCaráter da solicitação:'.");
            }
        }
        Console.WriteLine($"--- Extração de Exames Finalizada. Total de exames encontrados: {examesEncontrados.Count} ---");
        return examesEncontrados;
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
        if( texto.Contains(inicio))
        {
            var padrao = $@"{inicio}\s+(.*?)\s+{proximoCampo}";
            var match = Regex.Match(texto, padrao, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return match.Success ? LimparTexto(match.Groups[1].Value) : "";
        }else return "não extraiu";
    }
    private string ExtrairCampoCondicional(string texto, string inicio, List<string> proximoCampo)
    {
        var indexInicio = proximoCampo.IndexOf(inicio);
        if( texto.Contains(inicio))
        {
            if( indexInicio != -1)
            {
                for (int i = indexInicio + 1; i < proximoCampo.Count; i++)
                {
                    var proximoDelimitador = proximoCampo[i];

                    // Constrói o padrão regex: início do campo, qualquer coisa (não ganancioso), próximo delimitador.
                    // Usamos Lazy Quantifier `*?` para evitar capturar demais.
                    // var padrao = $@"{Regex.Escape(inicio)}\s*(.*?)\s*{Regex.Escape(proximoDelimitador)}";
                    // var padrao = $@"{Regex.Escape(inicio)}\s*(.*?)(?:\s*|){Regex.Escape(proximoDelimitador)}";
                    var padrao = $@"{Regex.Escape(inicio)}\s*(.*?)(?:\s+)?(?:{Regex.Escape(proximoDelimitador)})";
                    var match = Regex.Match(texto, padrao, RegexOptions.Singleline | RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        return LimparTexto(match.Groups[1].Value);
                    }else continue;

                }
                return "erro de campo final";
            }else return "erro na validação de inicio";
            // // Fallback: Se nenhum delimitador subsequente foi encontrado (ou se o campo de início não estava na lista)
            // // Tenta capturar o valor do campo até o próximo "título de campo" listado, ou até o final do texto.
            // // Isso é crucial para campos que são o último de uma seção ou onde o próximo delimitador está colado.

            // // Aprimoramento do fallback: tenta capturar o valor até o próximo delimitador *listado*
            // // que não seja o próprio inícioCampo, ou até o final da string.
            // var delimitadoresParaFallback = proximoCampo
            //     .Where(d => d != inicio) // Exclui o próprio campo de início
            //     .Select(Regex.Escape) // Escapa todos os delimitadores
            //     .ToList();

            // string regexDelimiters = string.Join("|", delimitadoresParaFallback);

            // // Regex para pegar tudo após o inícioCampo até o próximo delimitador na lista, ou até o final da string.
            // // (?= ... ) é um lookahead positivo, ele checa se algo está à frente mas não consome os caracteres.
            // // (?: ... ) é um grupo não-capturador.
            // // \s* : zero ou mais espaços
            // // $ : final da string
            // var padraoFallback = $@"{Regex.Escape(inicio)}\s*(.*?)(?=\s*(?:{regexDelimiters})|\s*$)";

            // var matchFallback = Regex.Match(texto, padraoFallback, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // if (matchFallback.Success)
            // {
            //     return LimparTexto(matchFallback.Groups[1].Value);
            // }

            // // Se nada funcionar, retorna vazio.
            // return "nada funcionou";
        }else return "Não informado";
    }

    private string ExtrairMarcado(string texto)
    {
        if (texto.Contains($"(X) Sim")) return "Sim";
        if (texto.Contains($"(X) Não")) return "Não";
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
    // private Prontuario ExtrairProntuarioViaAcroForm(AcroForm form)
    // {
    //     var prontuario = new Prontuario
    //     {
    //         DescricaoBasica = new DescricaoBasica
    //         {
    //             NomePaciente = GetValorCampo(form, "NomePaciente"),
    //             Idade = int.TryParse(GetValorCampo(form, "Idade"), out var idade) ? idade : 0,
    //             Profissao = GetValorCampo(form, "Profissao"),
    //             Religiao = GetValorCampo(form, "Religiao"),
    //             QD = GetValorCampo(form, "QD"),
    //             AtividadeFisica = GetValorCampo(form, "AtividadeFisica"),
    //             PacienteId = Paciente.ID
    //         },
    //         AGO = new AGO
    //         {
    //             Menarca = GetValorCampo(form, "Menarca"),
    //             DUM = GetValorCampo(form, "DUM"),
    //             Paridade = GetValorCampo(form, "Paridade"),
    //             DesejoGestacao = GetValorCampo(form, "DesejoGestacao"),
    //             CCO = GetValorCampo(form, "CCO"),
    //             MAC_TRH = GetValorCampo(form, "MAC_TRH"),
    //             VacinaHPV = StatusVacinaHPV.SemInfo // Ajustar se campo for checkbox
    //         },
    //         Antecedentes = new Antecedentes
    //         {
    //             Comorbidades = GetValorCampo(form, "Comorbidades"),
    //             Medicacao = GetValorCampo(form, "Medicacao"),
    //             Cirurgias = GetValorCampo(form, "Cirurgias"),
    //             Alergias = GetValorCampo(form, "Alergias"),
    //             Vicios = GetValorCampo(form, "Vicios"),
    //             HabitoIntestinal = GetValorCampo(form, "HabitoIntestinal"),
    //             Vacinas = GetValorCampo(form, "Vacinas")
    //         },
    //         AntecedentesFamiliares = new AntecedentesFamiliares
    //         {
    //             Neoplasias = GetValorCampo(form, "AF_Neoplasias"),
    //             Comorbidades = GetValorCampo(form, "AF_Comorbidades")
    //         },
    //         InformacoesExtras = "",
    //         CD = new List<AcoesCD>(), // Poderá ser mapeado via checkboxes
    //         Exames = new List<Exame>(), // Ainda a definir
    //         SolicitacaoInternacao = new Internacao(), // ainda a definir
    //         DataRequisicao = DateTime.Now
    //     };

    //     return prontuario;
    // }
    // private string GetValorCampo(AcroForm form, string nomeCampo)
    // {
    //     var campo = form.GetFields().FirstOrDefault(f =>
    //     string.Equals(f.FullyQualifiedName, nomeCampo, StringComparison.OrdinalIgnoreCase));

    //     return campo?.GetValue() ?? "";
    // }
    // private Dictionary<string, string> DividirPorSecoes(string texto)
    // {
    //     var secoes = new Dictionary<string, string>();

    //     string[] delimitadores = new[] {
    //         "Descrição básica", "AGO", "AP", "AF", "CD", "Pedido de Exame", "GUIA DE SOLICITAÇÃO"
    //     };

    //     for (int i = 0; i < delimitadores.Length; i++)
    //     {
    //         string inicio = delimitadores[i];
    //         string fim = i + 1 < delimitadores.Length ? delimitadores[i + 1] : null;

    //         int startIndex = texto.IndexOf(inicio, StringComparison.OrdinalIgnoreCase);
    //         if (startIndex == -1) continue;

    //         int endIndex = fim != null ? texto.IndexOf(fim, startIndex, StringComparison.OrdinalIgnoreCase) : texto.Length;

    //         if (endIndex == -1) endIndex = texto.Length;

    //         string bloco = texto.Substring(startIndex, endIndex - startIndex);
    //         secoes[inicio.ToLower()] = bloco;
    //     }

    //     return secoes;
    // }
}
