using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using DocumentFormat.OpenXml.Drawing;
using System.Globalization;
using HtmlAgilityPack;
using NPOI.SS.Formula.Functions;
using System.Text;
using System.Text.RegularExpressions;

namespace DocAPI.Services;
public class SenhaAutorizada
{
    public string NomePaciente { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public DateOnly DataPedido { get; set; } 
    public DateOnly? DataLibetracao { get; set; }
    public DateOnly? Validade { get; set; }
    public List<string>? Procedimento { get; set; }
    public string Status { get; set; }= string.Empty;
    public string Local { get; set; } = string.Empty;
}
public class ListaSenhas
{
    public DateTime? UltimaAtualizacao { get; set; }
    public List<SenhaAutorizada>? Senhas { get; set; }
}

public class FileDataOfSenhaExtractorService
{
    public string? PathToFile = @"C:\Users\lino\Projetos_Programação\doc_Organo\DocAPI\Secrets\SenhasAutorizadas\SenhasAtualizadas.json";
    private readonly CultureInfo _cultureInfo = new CultureInfo("pt-BR"); // Ou a cultura do seu XLSX/HTML
    public FileDataOfSenhaExtractorService(){}
    public FileDataOfSenhaExtractorService(string caminhoXls)
    {
        var fileTypeCheck = DetectarFormatoArquivo(caminhoXls);
        Console.WriteLine($"O arquivo é do tipo: {fileTypeCheck} , preparando extração de dados...");
        if(fileTypeCheck == ".html")
        {
            Console.WriteLine($"O arquivo é do tipo: {fileTypeCheck} , válido para extração de dados...");
        }
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

    public async Task<ListaSenhas> ExtractDataFromFileAsync(string filePath)
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
    public ListaSenhas ParseAndProcessHtml(string htmlContent)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        SenhaAutorizada SenhaPaciente = new SenhaAutorizada();
        var listaSenha = new ListaSenhas();

        // Seleciona todas as tabelas cujos IDs começam com "DataTables_Table_"
        // usando um seletor XPath mais genérico ou LINQ para filtrar os nós
        var dataTableNodes = htmlDoc.DocumentNode.SelectNodes("//table[starts-with(@id, 'DataTables_Table_0')]");

        if (dataTableNodes != null)
        {
            Console.WriteLine($"Encontradas {dataTableNodes.Count} tabelas com ID 'DataTables_Table_0'.");
            foreach (var tableNode in dataTableNodes)
            {
                var tableId = tableNode.Id; // Obtém o ID da tabela
                // Console.WriteLine($"Processando tabela com ID: {tableId}");

                listaSenha = ExtractTableData(htmlDoc, tableId);
            }
        } 
        else
        {
            Console.WriteLine("Nenhuma tabela com ID 'DataTables_Table_X' encontrada.");
        }
        return listaSenha;
    }
    private ListaSenhas ExtractTableData(HtmlDocument htmlDoc, string tableId)
    {
        ListaSenhas listaSenhas = new ListaSenhas();
        List<SenhaAutorizada> senhas = new List<SenhaAutorizada>();
        // Encontra a tabela pelo ID específico
        var tableNode = htmlDoc.DocumentNode.SelectSingleNode($"//table[@id='{tableId}']");
        if (tableNode != null)
        {
            var tbody = tableNode?.SelectSingleNode(".//tbody");
            if (tbody != null)
            {
                // Seleciona apenas as linhas que são efetivamente dados (têm 13 colunas <td>)
                // e que possuem o atributo 'role' como 'row' e classes 'odd' ou 'even'
                foreach (var row in tbody.SelectNodes(".//tr[count(td)=7 and @role='row' and (contains(@class, 'odd') or contains(@class, 'even'))]"))
                {
                    var cells = row.SelectNodes(".//td");
                    // Validação extra para garantir que todas as 13 células esperadas estão presentes
                    if (cells != null && cells.Count == 7)
                    {
                        try
                        {
                            // Console.WriteLine(cells[5].InnerHtml);
                            var senha = new SenhaAutorizada
                            {
                                DataLibetracao = ParseDateOnly(CleanHtmlText(cells[0].InnerHtml)),
                                Codigo = CleanHtmlText(cells[1].InnerHtml),
                                Validade = ParseDateOnly(CleanHtmlText(cells[2].InnerHtml)),
                                NomePaciente = CleanHtmlText(cells[3].InnerHtml),
                                Procedimento = OrganizeProcedimentos(CleanHtmlText(cells[4].InnerHtml)),
                                Status = CleanHtmlText(cells[5].InnerHtml),
                                Local = CleanHtmlText(cells[6].InnerHtml),
                            };
                            senhas.Add(senha);
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
                listaSenhas.Senhas = senhas;
                listaSenhas.UltimaAtualizacao = DateTime.Now;
            }
        }
        // Console.WriteLine($"Dados extraídos...Tipo de Guia:{listaDadosFinanceiros.TipoGuia}");
        return listaSenhas;
    }
    private string CleanHtmlText(string html)
    {
        if (string.IsNullOrEmpty(html)) return string.Empty;

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        // Usa InnerText para remover todas as tags e depois remove espaços indesejados e nbsp
        // Console.WriteLine($" clean: {doc.DocumentNode.InnerText}");
        
        var text = doc.DocumentNode.InnerText;
        
        text = text.Replace("&nbsp;", " ").Trim();
        text = Regex.Replace(text, @"\s+", " ", RegexOptions.Compiled).Trim();
        // Console.WriteLine($"  tratado: {doc.DocumentNode.InnerText}");
        return text;
    }
    private DateOnly? ParseDateOnly(string value) 
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Console.WriteLine("Aviso ParseDateOnly: String de data vazia ou nula.");
            return null;
        }

        DateOnly parsedDate;
        // Tente formatos específicos para robustez
        if (DateOnly.TryParseExact(value, "dd/MM/yyyy", _cultureInfo, DateTimeStyles.None, out parsedDate))
        {
            return parsedDate;
        }
        // Console.WriteLine($"Aviso ParseDateOnly: Não foi possível converter '{value}' para DateOnly (esperado 'dd/MM/yyyy'). Retornando null.");
        return null; // Retorna null em caso de falha
    }
    private List<string> OrganizeProcedimentos(string textoBruto)
    {
        var procedimentos = new List<string>();

        if (string.IsNullOrWhiteSpace(textoBruto))
        {
            return procedimentos; // Retorna lista vazia se a string for nula ou vazia
        }
        string pattern = @"(.*?)\s*-\s*qtd:(\d+)(?:\.\s*|$)";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Encontra todas as correspondências na string de entrada
        MatchCollection matches = regex.Matches(textoBruto);
        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            if (match.Success)
            {
                // Grupo 1 contém o nome do procedimento
                string nomeProcedimento = match.Groups[1].Value.Trim();
                procedimentos.Add(nomeProcedimento);
            }
        }
        return procedimentos;
    }
    public async Task<string> PrintSenhasExtraidosComoJson(ListaSenhas listaSenhas)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // para formatar bonitinho
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // estilo camelCase
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // Cria um MemoryStream para escrever o JSON de forma assíncrona
        using (var stream = new MemoryStream())
        {
            // Serializa os dados para o stream de forma assíncrona
            await JsonSerializer.SerializeAsync(stream, listaSenhas, options);

            // Volta a posição do stream para o início para que possamos ler o conteúdo
            stream.Seek(0, SeekOrigin.Begin);

            // Lê o conteúdo do stream para uma string de forma assíncrona
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
    public async Task SaveDescritivo(ListaSenhas listaSenhas)
    {
        Console.WriteLine("Salvando...");
        string baseDirectory = @"C:\Users\lino\Projetos_Programação\doc_Organo\DocAPI\Secrets\";
        string outputDirectory = System.IO.Path.Combine(baseDirectory, "SenhasAutorizadas");
        string senhasAtualizadasPath = System.IO.Path.Combine(outputDirectory, "SenhasAtualizadas.json");

        // OU (se você insiste neste caminho)
        // string senhasAtualizadasPath = @"C:\Users\lino\Projetos_Programação\doc_Organo\DocAPI\Secrets\DescrivoesFinanceiras\DescricaoFinanceira2025.json";
        // GARANTA QUE O DIRETÓRIO EXISTE ANTES DE QUALQUER OPERAÇÃO DE ARQUIVO
        string? directory = System.IO.Path.GetDirectoryName(senhasAtualizadasPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            try
            {
                Directory.CreateDirectory(directory);
                Console.WriteLine($"Diretório '{directory}' criado com sucesso.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.Error.WriteLine($"ERRO: Sem permissão para criar o diretório '{directory}'. Detalhes: {ex.Message}");
                Console.Error.WriteLine("Por favor, verifique as permissões da pasta ou mude o caminho de destino.");
                // return;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"ERRO: Não foi possível criar o diretório '{directory}'. Detalhes: {ex.Message}");
                //return;
            }
        }
        var senhasAtualizadasJson = await PrintSenhasExtraidosComoJson(listaSenhas);   
        File.WriteAllText(senhasAtualizadasPath, senhasAtualizadasJson);
        Console.WriteLine($"\nDados Salvos com sucesso e salvos em: {senhasAtualizadasPath}");
    }
    public async Task<ListaSenhas> LoadDescritivosFromFile(string filePath)
    {
        Console.WriteLine("Loading...");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Arquivo '{filePath}' não encontrado. Iniciando com uma nova lista vazia.");
            return new ListaSenhas();
        }
        try
        {
            string jsonContent = await File.ReadAllTextAsync(filePath);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                Console.WriteLine($"Aviso: Arquivo '{filePath}' está vazio ou contém apenas espaços em branco. Iniciando com uma nova lista vazia.");
                return new ListaSenhas();
            }
            
            using(var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent)))
            {
                // Agora, JsonSerializer.DeserializeAsync pode ler do stream
                // Você pode passar JsonSerializerOptions se precisar, como no PrintDadosExtraidosComoJson
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Mantenha a mesma política
                    // ... outras opções que você usa para serializar, se houver
                };

                var deserializedList = await JsonSerializer.DeserializeAsync<ListaSenhas>(stream, options);

            //var deserializedList = await JsonSerializer.DeserializeAsync<List<DescritivoFinanceiro>>(jsonContent);

                if (deserializedList == null)
                {
                    Console.WriteLine($"Aviso: Conteúdo do arquivo '{filePath}' resultou em lista nula após deserialização. Iniciando com uma nova lista vazia.");
                    return new ListaSenhas();
                }
                Console.WriteLine($"Verificando descrições loading... Última Atualização: {deserializedList.UltimaAtualizacao} Itens carregados: {deserializedList.Senhas?.Count}");
                Console.WriteLine("Verificando descripões loading..");
                return deserializedList;
            }
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"Erro ao deserializar o arquivo JSON '{filePath}': {ex.Message}. O arquivo pode estar corrompido ou mal formatado. Iniciando com uma nova lista vazia.");
            return new ListaSenhas();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Erro ao ler o arquivo '{filePath}': {ex.Message}. Iniciando com uma nova lista vazia.");
            return new ListaSenhas();
        }
    }
}