using Tabula;
using Tabula.Extractors;
using UglyToad.PdfPig;
using System.Text.Json;
using System.Text.RegularExpressions;
using DocAPI.Models;
using UglyToad.PdfPig.Graphics.Colors;

namespace DocAPI.Services;

public class PdfExtractorTabela
{
    public static void ExtrairExamesComCodigo()
    {
        string pathPdf = @"C:\Users\lino\Dropbox\io\Doc_Organo\ListaExamesUnimed.pdf";
        string jsonOutputPath = @"C:\Users\lino\Projetos_Programação\doc_Organo\Static/Exames.json";

        var exames = new List<Exame>();

        using (var document = PdfDocument.Open(pathPdf))
        {
            for (int i = 1; i <= document.NumberOfPages; i++)
            {
                var page = ObjectExtractor.Extract(document, i);
                var extractor = new SpreadsheetExtractionAlgorithm(); // Lattice mode (para tabelas bem definidas)
                var tables = extractor.Extract(page);

                foreach (var table in tables)
                {
                    foreach (var row in table.Rows)
                    {
                        if (row.Count < 3) continue; // Garante que tem as 3 colunas

                        var codigo = LimparTexto(row[1].ToString().Trim()) ;
                        var nome = row[2].ToString().Trim();

                        if (!string.IsNullOrWhiteSpace(codigo) && !string.IsNullOrWhiteSpace(nome))
                        {
                            exames.Add(new Exame
                            {
                                Codigo = codigo,
                                Nome = nome
                            });
                        }
                    }
                }
            }
        }

        var json = JsonSerializer.Serialize(exames, new JsonSerializerOptions 
        { 
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
        File.WriteAllText(jsonOutputPath, json);
        Console.WriteLine($"Extraídos {exames.Count} exames com códigos. Salvo em: {jsonOutputPath}");
    }
    private static string LimparTexto(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";

        // Remove quebras de linha, tabulação, espaços extras
        input = Regex.Replace(input, @"\r\n?|\n", " "); // quebra de linha → espaço
        input = Regex.Replace(input, @"\t", " ");       // tab → espaço
        input = Regex.Replace(input, @"\s{2,}", " ");   // múltiplos espaços → um espaço

        return input.Trim();
    }
}
