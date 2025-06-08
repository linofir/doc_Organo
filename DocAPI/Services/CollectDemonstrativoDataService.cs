using ClosedXML;
using ClosedXML.Excel;
using NPOI.HSSF.UserModel;  // Para .xls
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // Para .xlsx
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using DocumentFormat.OpenXml.Drawing;
using System.Globalization;
using HtmlAgilityPack;
using NPOI.SS.Formula.Functions;

namespace DocAPI.Services;
public class DadosFinanceiros
{
    public string? Lote { get; set; } // Propriedades devem ser com { get; set; }
    public string? Guia { get; set; }
    public string? Protocolo { get; set; }
    public string? NomeUsuario { get; set; }
    public string? TipoUsuario { get; set; }
    public string? InfoP { get; set; }
    public DateOnly Data { get; set; } // Ajustado para DateOnly
    public string? Servico { get; set; }
    public string? ServicoCodigo { get; set; }
    public int? Quantidade { get; set; }
    public decimal? ValorTabela { get; set; } // RECOMENDADO: Alterar para decimal?
    public string? GrauParticipacao { get; set; }
    public decimal? PercentualVia { get; set; } // RECOMENDADO: Alterar para decimal?
    public string? HonorarioFator { get; set; } // RECOMENDADO: Alterar para decimal?
    public decimal? ValorPago { get; set; } // RECOMENDADO: Alterar para decimal?
}
public class ListaDados
{
    public string? TipoGuia { get; set; }
    public List<DadosFinanceiros>? Dados { get; set; }
}

public class FileDataExtractorService
{
    private string? PathToFile;
    private readonly CultureInfo _cultureInfo = new CultureInfo("pt-BR"); // Ou a cultura do seu XLSX/HTML

    public FileDataExtractorService(string caminhoXls)
    {
        var fileTypeCheck = DetectarFormatoArquivo(caminhoXls);
        Console.WriteLine($"O arquivo é do tipo: {fileTypeCheck} , preparando extração de dados...");
        if(fileTypeCheck == ".html")
        {
            Console.WriteLine($"O arquivo é do tipo: {fileTypeCheck} , válido para extração de dados...");

        }
        // string novoCaminho = @"C:\Users\lino\Dropbox\io\Doc_Organo\Descricao.xlsx";
        // string resultado = CopiarOuConverterParaXlsx(caminhoXls, novoCaminho);
        // string caminhoPreparado = PrepararArquivoParaExtracao(caminhoXls, novoCaminho);
        // Console.WriteLine($"Arquivo pronto para leitura: {caminhoPreparado}");
    }
    public string DetectarFormatoArquivo(string caminho)
    {
        byte[] buffer = new byte[8];

        using (var fs = new FileStream(caminho, FileMode.Open, FileAccess.Read))
        {
            fs.Read(buffer, 0, buffer.Length);
        }

        string headerHex = BitConverter.ToString(buffer).Replace("-", "").ToUpperInvariant();

        // Formatos conhecidos
        if (headerHex.StartsWith("504B0304"))
            return ".xlsx (ZIP - OpenXML)";

        if (headerHex.StartsWith("D0CF11E0A1B11AE1"))
            return ".xls (OLE2 binary)";

        if (headerHex.StartsWith("3C68746D6C") || headerHex.StartsWith("3C48544D4C")) // "<html"
            return ".html";

        if (headerHex.StartsWith("FFFE") || headerHex.StartsWith("FEFF") || headerHex.StartsWith("EFBBBF"))
            return ".csv ou .txt com BOM";

        if (headerHex.StartsWith("49492A00") || headerHex.StartsWith("4D4D002A"))
            return "TIFF (imagem?)";

        if (headerHex.StartsWith("25504446"))
            return "PDF";

        return $"Desconhecido (Header: {headerHex})";
    }
    public async Task<List<ListaDados>> ExtractDataFromFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"O arquivo não foi encontrado: {filePath}");
        }

        string htmlContent;
        try
        {
            htmlContent = await File.ReadAllTextAsync(filePath);
            Console.WriteLine($"Conteúdo HTML lido do arquivo: {filePath}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Erro ao ler o arquivo {filePath}: {ex.Message}");
            throw;
        }
        // Console.WriteLine("--- Conteúdo HTML lido (primeiras 500 caracteres) ---");
        // Console.WriteLine(htmlContent.Substring(0, Math.Min(htmlContent.Length, 500))); // Imprime o início do HTML
        // Console.WriteLine("--- Fim do Conteúdo HTML ---");
        return ParseAndProcessHtml(htmlContent);
    }
    public List<ListaDados> ParseAndProcessHtml(string htmlContent)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        var allCategorizedData = new List<ListaDados>();

        // Seleciona todas as tabelas cujos IDs começam com "DataTables_Table_"
        // usando um seletor XPath mais genérico ou LINQ para filtrar os nós
        var dataTableNodes = htmlDoc.DocumentNode.SelectNodes("//table[starts-with(@id, 'DataTables_Table_')]");

        if (dataTableNodes != null)
        {
            Console.WriteLine($"Encontradas {dataTableNodes.Count} tabelas com ID 'DataTables_Table_X'.");
            foreach (var tableNode in dataTableNodes)
            {
                var tableId = tableNode.Id; // Obtém o ID da tabela
                Console.WriteLine($"Processando tabela com ID: {tableId}");

                allCategorizedData.Add(ExtractTableData(htmlDoc, tableId));
            }
        } 
        else
        {
            Console.WriteLine("Nenhuma tabela com ID 'DataTables_Table_X' encontrada.");
        }

        return allCategorizedData;
    }
    private ListaDados ExtractTableData(HtmlDocument htmlDoc, string tableId)
    {
        ListaDados listaDadosFinanceiros = new ListaDados();
        List<DadosFinanceiros> dadosFinanceiros = new List<DadosFinanceiros>();
        // Encontra a tabela pelo ID específico
        var tableNode = htmlDoc.DocumentNode.SelectSingleNode($"//table[@id='{tableId}']");
        if (tableNode != null)
        {
            //Procurar Dentro do Thead o tipo 
            var thead = tableNode.SelectSingleNode(".//thead");
            if(thead != null)
            {
                Console.WriteLine($"thead encontrado..");
                var headerTds = thead.SelectNodes(".//tr");
                var lastTrInThead = thead.SelectNodes(".//tr")?.LastOrDefault();
                if( lastTrInThead != null)
                {
                    var tdWithTipoGuia = lastTrInThead.SelectSingleNode(".//td[contains(., 'Tipo de guia:')]");

                        if (tdWithTipoGuia != null)
                        {
                            Console.WriteLine("Encontrou o campo tipo");
                            string fullText = CleanHtmlText(tdWithTipoGuia.InnerHtml);
                            const string searchString = "Tipo de guia:";
                            int startIndex = fullText.IndexOf(searchString, StringComparison.OrdinalIgnoreCase);

                            if(startIndex != -1)
                            {
                                string tipoGuiaValue = fullText.Substring(startIndex + searchString.Length).Trim();
                                listaDadosFinanceiros.TipoGuia = tipoGuiaValue;
                                Console.WriteLine($"Tipo de Guia encontrado para tabela {tableId}: {tipoGuiaValue}");
                            }else
                            {
                                Console.WriteLine($"Aviso: 'Tipo de guia:' texto não encontrado na célula esperada no último tr do thead para tabela {tableId}.");
                                listaDadosFinanceiros.TipoGuia = $"Tipo Desconhecido (Tabela {tableId.Replace("DataTables_Table_", "")})";
                            }
                        }
                    else
                    {
                        Console.WriteLine($"Aviso: Célula contendo 'Tipo de guia:' não encontrada no último tr do thead para tabela {tableId}.");
                        listaDadosFinanceiros.TipoGuia = $"Tipo Desconhecido (Tabela {tableId.Replace("DataTables_Table_", "")})";
                    }
                }
                else
                {
                    Console.WriteLine($"Aviso: Último tr no thead não encontrado para tabela {tableId}.");
                    listaDadosFinanceiros.TipoGuia = $"Tipo Desconhecido (Tabela {tableId.Replace("DataTables_Table_", "")})";
                }
            }
            else
            {
                Console.WriteLine($"Aviso: Thead não encontrado para tabela {tableId}.");
                listaDadosFinanceiros.TipoGuia = $"Tipo Desconhecido (Tabela {tableId.Replace("DataTables_Table_", "")})";
            }
                //         }
                // }
                // if (headerTds != null && headerTds.Count >= 3)
                // {
                //     var tipoDeGuiaValue = headerTds[2].InnerText.Trim(); // O terceiro td (índice 2)
                //     listaDadosFinanceiros.TipoGuia = tipoDeGuiaValue;

                // }
                
            var tbody = tableNode?.SelectSingleNode(".//tbody");
            if (tbody != null)
            {
                // Seleciona apenas as linhas que são efetivamente dados (têm 13 colunas <td>)
                // e que possuem o atributo 'role' como 'row' e classes 'odd' ou 'even'
                foreach (var row in tbody.SelectNodes(".//tr[count(td)=13 and @role='row' and (contains(@class, 'odd') or contains(@class, 'even'))]"))
                {
                    var cells = row.SelectNodes(".//td");
                    // Validação extra para garantir que todas as 13 células esperadas estão presentes
                    if (cells != null && cells.Count == 13)
                    {
                        try
                        {
                            string loteGuiaRaw = cells[0].InnerText.Trim();
                            var loteGuiaParts = loteGuiaRaw.Split(new[] { '-' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    
                            string servicoRaw = cells[6].InnerText.Trim();
                            // O HTML mostra <div><span>&nbsp;</span>31301134-VULVECTOMIA SIMPLES</div>
                            // O InnerText pegará o " 31301134-VULVECTOMIA SIMPLES". Precisa de trim para remover o nbsp
                            servicoRaw = servicoRaw.Replace("&nbsp;", "").Trim(); // Remove o &nbsp; e trim
                            var servicoParts = servicoRaw.Split(new[] { '-' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    
                            var dado = new DadosFinanceiros
                            {
                                Lote = loteGuiaParts.Length > 0 ? loteGuiaParts[0].Trim() : null,
                                Guia = loteGuiaParts.Length > 1 ? loteGuiaParts[1].Trim() : null,
                                Protocolo = CleanHtmlText(cells[1].InnerHtml),
                                NomeUsuario = CleanHtmlText(cells[2].InnerHtml),
                                TipoUsuario = CleanHtmlText(cells[3].InnerHtml),
                                InfoP = CleanHtmlText(cells[4].InnerHtml),
                                Data = ParseDateOnly(cells[5].InnerHtml),
                                Servico = servicoParts.Length > 0 ? servicoParts[0].Trim() : null,
                                ServicoCodigo = servicoParts.Length > 1 ? servicoParts[1].Trim() : null,
                                Quantidade = ParseInt(CleanHtmlText(cells[7].InnerHtml)),
                                ValorTabela = ParseDecimal(CleanHtmlText(cells[8].InnerHtml)),
                                GrauParticipacao = CleanHtmlText(CleanHtmlText(cells[9].InnerHtml)),
                                PercentualVia = ParseDecimal(CleanHtmlText(cells[10].InnerHtml)),
                                HonorarioFator = CleanHtmlText(CleanHtmlText(cells[11].InnerHtml)),
                                ValorPago = ParseDecimal(CleanHtmlText(cells[12].InnerHtml).Replace("+", "").Trim())
                            };
                            // dadosFinanceiros.Add(dado);
                            dadosFinanceiros.Add(dado);
                        }
                        catch (FormatException fe)
                        {
                            Console.Error.WriteLine($"Erro de formato ao parsear dados na tabela '{tableId}', linha: {row.InnerHtml}. Erro: {fe.Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"Erro inesperado ao parsear dados na tabela '{tableId}', linha: {row.InnerHtml}. Erro: {ex.Message}");
                        }
                    }
                }
                listaDadosFinanceiros.Dados = dadosFinanceiros;
            }
        }
        return listaDadosFinanceiros;
    }
    private string CleanHtmlText(string html)
    {
        if (string.IsNullOrEmpty(html)) return string.Empty;

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        // Usa InnerText para remover todas as tags e depois remove espaços indesejados e nbsp
        return doc.DocumentNode.InnerText.Replace("&nbsp;", " ").Trim();
    }
    private int? ParseInt(string value)
    {
        if (int.TryParse(value, NumberStyles.Any, _cultureInfo, out int result))
        {
            return result;
        }
        Console.WriteLine($"Aviso: Não foi possível converter '{value}' para inteiro. Retornando null.");
        return null;
    }

    private decimal? ParseDecimal(string value)
    {
        // Crie uma CultureInfo específica para o português do Brasil
        // Esta cultura entende '.' como separador de milhar e ',' como separador decimal.
        CultureInfo ptBRCulture = new CultureInfo("pt-BR");

        // Remove espaços em branco do início e fim da string
        string cleanedValue = value.Trim();

        // Tenta parsear usando a cultura pt-BR e estilos que permitem números e moedas.
        // NumberStyles.Currency é útil se a string puder conter símbolos de moeda (R$)
        // NumberStyles.Number lida com números gerais com separadores e sinais.
        if (decimal.TryParse(cleanedValue, NumberStyles.Currency | NumberStyles.Number, ptBRCulture, out decimal result))
        {
            return result;
        }
        Console.Error.WriteLine($"Aviso: Não foi possível converter '{value}' para decimal. Retornando null.");
        return null; // Retorna null para valores inválidos
        // Normaliza o valor: substitui ponto por vírgula para pt-BR
        // string normalizedValue = value.Replace('.', ',').Trim();

        // if (decimal.TryParse(normalizedValue, NumberStyles.Currency | NumberStyles.Number, _cultureInfo, out decimal result))
        // {
        //     return result;
        // }
        // Console.WriteLine($"Aviso: Não foi possível converter '{value}' para decimal. Retornando null.");
        // return null;
    }

    private DateOnly ParseDateOnly(string value)
    {
        if (DateTime.TryParseExact(value, "dd/MM/yyyy", _cultureInfo, DateTimeStyles.None, out DateTime dateTimeResult))
        {
            return DateOnly.FromDateTime(dateTimeResult);
        }
        Console.WriteLine($"Aviso: Não foi possível converter '{value}' para DateOnly (esperado 'dd/MM/yyyy'). Usando DateOnly.MinValue.");
        return DateOnly.MinValue; // Ou você pode retornar null se a propriedade Data for DateOnly?
    }

/// ----------------------------------
    public string PrepararArquivoParaExtracao(string caminhoOriginal, string caminhoXlsxDestino)
    {
        try
        {
            // Tenta abrir como .xlsx usando ClosedXML
            using (var workbook = new XLWorkbook(caminhoOriginal))
            {
                // Sucesso na leitura, é um xlsx válido
                File.Copy(caminhoOriginal, caminhoXlsxDestino, overwrite: true);
                Console.WriteLine($"✅ Arquivo já era .xlsx. Cópia salva em: {caminhoXlsxDestino}");
                PathToFile = caminhoXlsxDestino;
                return caminhoXlsxDestino;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ O arquivo não é um .xlsx válido. Tentando conversão como .xls... ({ex.Message})");

            // Tenta converter como .xls (binário)
            ConverterXlsParaXlsx(caminhoOriginal, caminhoXlsxDestino);
            Console.WriteLine($"✅ Arquivo convertido para .xlsx: {caminhoXlsxDestino}");
            PathToFile = caminhoXlsxDestino;
            return caminhoXlsxDestino;
        }
    }    
    public void ConverterXlsParaXlsx(string caminhoXls, string caminhoXlsx)
    {
        using var fileStream = new FileStream(caminhoXls, FileMode.Open, FileAccess.Read);
        var hssfWorkbook = new HSSFWorkbook(fileStream); // Leitura do .xls

        var xssfWorkbook = new XSSFWorkbook(); // Criação do novo .xlsx

        for (int i = 0; i < hssfWorkbook.NumberOfSheets; i++)
        {
            var oldSheet = hssfWorkbook.GetSheetAt(i);
            var newSheet = xssfWorkbook.CreateSheet(oldSheet.SheetName);

            for (int rowIndex = 0; rowIndex <= oldSheet.LastRowNum; rowIndex++)
            {
                var oldRow = oldSheet.GetRow(rowIndex);
                if (oldRow == null) continue;

                var newRow = newSheet.CreateRow(rowIndex);

                for (int col = 0; col < oldRow.LastCellNum; col++)
                {
                    var oldCell = oldRow.GetCell(col);
                    if (oldCell == null) continue;

                    var newCell = newRow.CreateCell(col);

                    switch (oldCell.CellType)
                    {
                        case CellType.String:
                            newCell.SetCellValue(oldCell.StringCellValue);
                            break;
                        case CellType.Numeric:
                            if (DateUtil.IsCellDateFormatted(oldCell))
                            {
                                var dateValue = oldCell.DateCellValue;
                                if (dateValue.HasValue)
                                    newCell.SetCellValue(dateValue.Value);
                                else
                                    newCell.SetCellValue(""); // ou: newCell.SetCellValue("Data inválida");
                            }
                            else
                            {
                                newCell.SetCellValue(oldCell.NumericCellValue);
                            }
                            break;
                        case CellType.Boolean:
                            newCell.SetCellValue(oldCell.BooleanCellValue);
                            break;
                        case CellType.Formula:
                            try
                            {
                                switch (oldCell.CachedFormulaResultType)
                                {
                                    case CellType.Numeric:
                                        newCell.SetCellValue(oldCell.NumericCellValue);
                                        break;
                                    case CellType.String:
                                        newCell.SetCellValue(oldCell.StringCellValue);
                                        break;
                                    case CellType.Boolean:
                                        newCell.SetCellValue(oldCell.BooleanCellValue);
                                        break;
                                    default:
                                        newCell.SetCellValue(oldCell.ToString());
                                        break;
                                }
                            }
                            catch
                            {
                                newCell.SetCellValue(oldCell.ToString());
                            }
                            break;
                        default:
                            newCell.SetCellValue(oldCell.ToString());
                            break;
                    }
                }
            }
        }

    using var outFile = new FileStream(caminhoXlsx, FileMode.Create, FileAccess.Write);
    xssfWorkbook.Write(outFile);
}
    
    public List<ListaDados> ExtrairDadosFinanceiros()
    {
        var listaCompleta = new List<ListaDados>();
        using var workbook = new XLWorkbook(PathToFile);
        var worksheet = workbook.Worksheet(1);

        ListaDados? listaAtual = null;

        foreach (var row in worksheet.RowsUsed())
        {
            var firstCell = row.Cell(1).GetString();

            // Detecta início de novo grupo (tipo de guia)
            if (firstCell.StartsWith("Tipo de Guia", StringComparison.OrdinalIgnoreCase))
            {
                string tipo = firstCell.Split(":").Last().Trim();
                listaAtual = new ListaDados
                {
                    TipoGuia = tipo,
                    Dados = new List<DadosFinanceiros>()
                };
                listaCompleta.Add(listaAtual);
                continue;
            }

            // Pula linhas com cabeçalhos ou vazias
            if (string.IsNullOrWhiteSpace(firstCell) || row.Cell(1).Address.RowNumber == 1)
                continue;

            if (listaAtual == null) continue; // Segurança

            var dado = new DadosFinanceiros();

            try
            {
                // Lote - Guia
                var loteGuia = row.Cell("A").GetString().Split('-');
                dado.Lote = loteGuia.Length > 0 ? loteGuia[0].Trim() : null;
                dado.Guia = loteGuia.Length > 1 ? loteGuia[1].Trim() : null;

                dado.Protocolo = row.Cell("B").GetString();
                dado.NomeUsuario = row.Cell("C").GetString();
                dado.TipoUsuario = row.Cell("U").GetString();  // Coluna U = 21
                dado.InfoP = row.Cell("P").GetString();        // Coluna P = 16

                // Data
                var cellValue = row.Cell("F").Value;

                if (cellValue.Type == XLDataType.DateTime && cellValue.GetDateTime() is DateTime dt)
                {
                    dado.Data = DateOnly.FromDateTime(dt);
                }
                else if (DateTime.TryParse(row.Cell("F").GetString(), out var parsed))
                {
                    dado.Data = DateOnly.FromDateTime(parsed);
                }

                // Serviço
                var servicoSplit = row.Cell("G").GetString().Split('-');
                dado.ServicoCodigo = servicoSplit.Length > 0 ? servicoSplit[0].Trim() : null;
                dado.Servico = servicoSplit.Length > 1 ? servicoSplit[1].Trim() : null;

                // Outras colunas
                dado.Quantidade = row.Cell("H").TryGetValue<int>(out var qtd) ? qtd : null;
                dado.ValorTabela = Decimal.Parse(row.Cell("I").GetString());
                dado.GrauParticipacao = row.Cell("X").GetString(); // GP
                dado.PercentualVia = Decimal.Parse(row.Cell("Y").GetString());    // PV
                dado.HonorarioFator = row.Cell("Z").GetString();   // AHE
                dado.ValorPago = Decimal.Parse(row.Cell("J").GetString());

                listaAtual.Dados?.Add(dado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar linha {row.RowNumber()}: {ex.Message}");
            }
        }

        return listaCompleta;
    }
    public string PrintDadosExtraidosComoJson(List<ListaDados> dadosExtraidos)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // para formatar bonitinho
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // estilo camelCase
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return JsonSerializer.Serialize(dadosExtraidos, options);
    }
    
}
public class HtmlDataExtractorService
{
    private readonly HttpClient _httpClient;
    private readonly CultureInfo _cultureInfo = new CultureInfo("pt-BR"); // Ou a cultura do seu XLSX/HTML

    public HtmlDataExtractorService(HttpClient httpClient )
    {
        _httpClient = httpClient;
    }
  
    /// <summary>
    /// Extrai dados de uma URL, baixando o conteúdo HTML.
    /// </summary>
    /// <param name="url">URL da página HTML.</param>
    /// <returns>Uma lista de objetos ListaDados, onde cada ListaDados representa um tipo de guia.</returns>
    public async Task<List<ListaDados>> ExtractDataFromUrlAsync(string url)
    {
        string htmlContent;
        try
        {
            htmlContent = await _httpClient.GetStringAsync(url);
            Console.WriteLine($"Conteúdo HTML baixado da URL: {url}");
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"Erro ao baixar conteúdo da URL {url}: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Erro inesperado ao baixar da URL {url}: {ex.Message}");
            throw;
        }

        return ParseAndProcessHtml(htmlContent);
    }
    /// <summary>
    /// Método central para parsear o HTML e extrair os dados.
    /// </summary>
    /// <param name="htmlContent">A string contendo o HTML a ser parseado.</param>
    /// <returns>Uma lista de objetos ListaDados, onde cada ListaDados representa um tipo de guia.</returns>
    public List<ListaDados> ParseAndProcessHtml(string htmlContent)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        var allCategorizedData = new List<ListaDados>();

        // 1. Encontrar a tabela principal pelo ID "tabelaExportada"
        var mainTableNode = htmlDoc.DocumentNode.SelectSingleNode("//table[@id='tabelaExportada']");

        if (mainTableNode == null)
        {
            Console.WriteLine("Erro: Tabela principal com ID 'tabelaExportada' não encontrada.");
            return allCategorizedData;
        }

        // 2. Navegar para o tbody da tabela principal
        var mainTbodyNode = mainTableNode.SelectSingleNode(".//tbody"); // .// para procurar dentro do nó atual

        if (mainTbodyNode == null)
        {
            Console.WriteLine("Erro: tbody da tabela principal não encontrado.");
            return allCategorizedData;
        }

        // 3. Iterar sobre os trs dentro do tbody principal para encontrar as tabelas aninhadas
        // O user mencionou "Em uma dessas trs... <table width="80%"...<tbody><tr>".
        // Isso sugere que as tabelas de dados estão dentro de TDs de TRs específicos dentro do tbody principal.
        // Vamos procurar por todas as tabelas com ID "DataTables_Table_X" em qualquer lugar dentro do mainTbodyNode.
        var nestedTableNodes = mainTbodyNode.SelectNodes(".//table[starts-with(@id, 'DataTables_Table_')]");

        if (nestedTableNodes == null || !nestedTableNodes.Any())
        {
            Console.WriteLine("Nenhuma tabela aninhada com ID 'DataTables_Table_X' encontrada.");
            return allCategorizedData;
        }

        foreach (var currentDataTable in nestedTableNodes)
        {
            string tipoDeGuiaValue = string.Empty;
            var currentListaDados = new ListaDados();

            // 4. No thead da tabela aninhada, no terceiro td, tem o Tipo da lista
            var theadNode = currentDataTable.SelectSingleNode(".//thead");
            if (theadNode != null)
            {
                var headerTds = theadNode.SelectNodes(".//tr/td");
                if (headerTds != null && headerTds.Count >= 3)
                {
                    tipoDeGuiaValue = headerTds[2].InnerText.Trim(); // O terceiro td (índice 2)
                    currentListaDados.TipoGuia = tipoDeGuiaValue;
                }
                else
                {
                    Console.WriteLine($"Aviso: Não foi possível encontrar o 'Tipo de Guia' para a tabela {currentDataTable.Id}.");
                }
            }

            if (string.IsNullOrWhiteSpace(tipoDeGuiaValue))
            {
                Console.WriteLine($"Aviso: Tipo de Guia vazio ou não encontrado para a tabela {currentDataTable.Id}. Ignorando esta tabela.");
                continue; // Pula para a próxima tabela se o tipo não for encontrado
            }

            // 5. No tbody da tabela aninhada, extrair os dados de cada tr
            var tbodyNode = currentDataTable.SelectSingleNode(".//tbody");
            if (tbodyNode == null)
            {
                Console.WriteLine($"Aviso: tbody não encontrado para a tabela {currentDataTable.Id}.");
                continue;
            }

            var dataRows = tbodyNode.SelectNodes(".//tr");

            if (dataRows == null || !dataRows.Any())
            {
                Console.WriteLine($"Nenhuma linha de dados encontrada na tabela {currentDataTable.Id}.");
                continue;
            }

            // Mapeamento dos nomes das colunas para seus índices (assumindo a mesma ordem para todas as tabelas de dados)
            // Os cabeçalhos de cada tabela de dados são geralmente consistentes.
            // Para as tabelas DataTables_Table_X, os cabeçalhos são fixos.
            // O usuário descreveu a estrutura de uma TR, então vamos mapear por índice.
            // Se o thead da *tabela de dados* tiver cabeçalhos TH, poderíamos usar um columnMap dinâmico para cada tabela de dados.
            // Mas, dado que a estrutura da TR é fixa, vamos usar índices fixos.

            // Indices baseados na TR de exemplo fornecida pelo usuário:
            // <td class="grid-border-right sorting_1" nowrap="" style="width:60px;">1177927 - 173559409</td>           -> Lote - Guia (0)
            // <td class="grid-border-right" nowrap="" style="width:60px;">1151303</td>                                  -> Protocolo (1)
            // <td class="grid-border-right">...DANIELE SIQUEIRA HESSEL...</td>                                     -> NomeUsuario (2)
            // <td class="grid-border-right text-center">UF</td>                                                          -> TipoUsuario (U) (3)
            // <td class="grid-border-right text-center">E</td>                                                            -> InfoP (P) (4)
            // <td class="grid-border-right text-center">10/04/2025</td>                                                  -> Data (5)
            // <td class="grid-border-right" style="max-width:180px;">...31301134-VULVECTOMIA SIMPLES...</td>             -> Serviço (6)
            // <td class="grid-border-right text-center">1</td>                                                          -> Quantidade (7)
            // <td class="grid-border-right text-right">1.462,50</td>                                                   -> ValorTabela (8)
            // <td class="grid-border-right text-center">1</td>                                                          -> GrauParticipacao (GP) (9)
            // <td class="grid-border-right text-center">1</td>                                                          -> PercentualVia (PV) (10)
            // <td class="grid-border-right text-center">1</td>                                                          -> HonorarioFator (AHE) (11)
            // <td style="text-align:right;color:#006600;" nowrap="">1.462,50 +</td>                                     -> ValorPago (12)

            foreach (var row in dataRows)
            {
                var cells = row.SelectNodes(".//td");
                if (cells == null || cells.Count < 13) // Mínimo de colunas esperadas
                {
                    Console.WriteLine($"Aviso: Linha de dados incompleta ou sem células na tabela {currentDataTable.Id} (HTML linha {row.Line}). Ignorando.");
                    continue;
                }

                try
                {
                    var dadosFinanceiros = new DadosFinanceiros();

                    // Lote e Guia (coluna 0)
                    string loteGuiaRaw = cells[0].InnerText.Trim();
                    var loteGuiaParts = loteGuiaRaw.Split(new[] { '-' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    dadosFinanceiros.Lote = loteGuiaParts.Length > 0 ? loteGuiaParts[0].Trim() : null;
                    dadosFinanceiros.Guia = loteGuiaParts.Length > 1 ? loteGuiaParts[1].Trim() : null;

                    dadosFinanceiros.Protocolo = cells[1].InnerText.Trim(); // Coluna 1
                    dadosFinanceiros.NomeUsuario = cells[2].InnerText.Trim(); // Coluna 2
                    dadosFinanceiros.TipoUsuario = cells[3].InnerText.Trim(); // Coluna 3 (U)
                    dadosFinanceiros.InfoP = cells[4].InnerText.Trim(); // Coluna 4 (P)
                    dadosFinanceiros.Data = ParseDateOnly(cells[5].InnerText.Trim()); // Coluna 5

                    // Serviço e Serviço Código (coluna 6)
                
                    string servicoRaw = cells[6].InnerText.Trim();
                    // O HTML mostra <div><span>&nbsp;</span>31301134-VULVECTOMIA SIMPLES</div>
                    // O InnerText pegará o " 31301134-VULVECTOMIA SIMPLES". Precisa de trim para remover o nbsp
                    servicoRaw = servicoRaw.Replace("&nbsp;", "").Trim(); // Remove o &nbsp; e trim
                    var servicoParts = servicoRaw.Split(new[] { '-' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    dadosFinanceiros.ServicoCodigo = servicoParts.Length > 0 ? servicoParts[0].Trim() : null;
                    dadosFinanceiros.Servico = servicoParts.Length > 1 ? servicoParts[1].Trim() : null;

                    dadosFinanceiros.Quantidade = ParseInt(cells[7].InnerText.Trim()); // Coluna 7
                    dadosFinanceiros.ValorTabela = ParseDecimal(cells[8].InnerText.Trim()); // Coluna 8
                    dadosFinanceiros.GrauParticipacao = cells[9].InnerText.Trim(); // Coluna 9 (GP)
                    dadosFinanceiros.PercentualVia = ParseDecimal(cells[10].InnerText.Trim()); // Coluna 10 (PV)
                    dadosFinanceiros.HonorarioFator = cells[11].InnerText.Trim(); // Coluna 11 (AHE)
                    dadosFinanceiros.ValorPago = ParseDecimal(cells[12].InnerText.Trim().Replace("+", "")); // Coluna 12, remover o '+'

                    currentListaDados.Dados?.Add(dadosFinanceiros);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Erro ao processar linha de dados na tabela {currentDataTable.Id} (HTML linha {row.Line}): {ex.Message}");
                    // Logar e continuar ou relançar.
                }
            }
            allCategorizedData.Add(currentListaDados);
        }

        Console.WriteLine("Parsing HTML concluído.");
        return allCategorizedData;
    }

    // --- Métodos Auxiliares para Leitura e Conversão de Células ---
    // Ajustei para serem mais robustos com valores nulos e mensagens de log.

    private int? ParseInt(string value)
    {
        if (int.TryParse(value, NumberStyles.Any, _cultureInfo, out int result))
        {
            return result;
        }
        Console.WriteLine($"Aviso: Não foi possível converter '{value}' para inteiro. Retornando null.");
        return null;
    }

    private decimal? ParseDecimal(string value)
    {
        // Normaliza o valor: substitui ponto por vírgula para pt-BR
        string normalizedValue = value.Replace('.', ',').Trim();

        if (decimal.TryParse(normalizedValue, NumberStyles.Currency | NumberStyles.Number, _cultureInfo, out decimal result))
        {
            return result;
        }
        Console.WriteLine($"Aviso: Não foi possível converter '{value}' para decimal. Retornando null.");
        return null;
    }

    private DateOnly ParseDateOnly(string value)
    {
        if (DateTime.TryParseExact(value, "dd/MM/yyyy", _cultureInfo, DateTimeStyles.None, out DateTime dateTimeResult))
        {
            return DateOnly.FromDateTime(dateTimeResult);
        }
        Console.WriteLine($"Aviso: Não foi possível converter '{value}' para DateOnly (esperado 'dd/MM/yyyy'). Usando DateOnly.MinValue.");
        return DateOnly.MinValue; // Ou você pode retornar null se a propriedade Data for DateOnly?
    }
    public string PrintDadosExtraidosComoJson(List<ListaDados> dadosExtraidos)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // para formatar bonitinho
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // estilo camelCase
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return JsonSerializer.Serialize(dadosExtraidos, options);
    }
}