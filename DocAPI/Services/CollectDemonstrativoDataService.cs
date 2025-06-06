using ClosedXML;
using ClosedXML.Excel;
using NPOI.HSSF.UserModel;  // Para .xls
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // Para .xlsx
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DocAPI.Services;
public class CollectDemonstrativoDataService
{
    public CollectDemonstrativoDataService(string caminhoXls)
    {
        Console.WriteLine(DetectarFormatoArquivo(caminhoXls));
        string novoCaminho = @"C:\Users\lino\Dropbox\io\Doc_Organo\Descricao.xlsx";
        // string resultado = CopiarOuConverterParaXlsx(caminhoXls, novoCaminho);
        string caminhoPreparado = PrepararArquivoParaExtracao(caminhoXls, novoCaminho);
        Console.WriteLine($"Arquivo pronto para leitura: {caminhoPreparado}");
    }
    private string? PathToFile;
    public class DadosFinanceiros
    {
        public string? Lote;
        public string? Guia;
        public string? Protocolo;
        public string? NomeUsuario;
        public string? TipoUsuario;
        public string? InfoP;
        public DateOnly Data;
        public string? Servico;
        public string? ServicoCodigo;
        public int? Quantidade;
        public string? ValorTabela;
        public string? GrauParticipacao;
        public string? PercentualVia;
        public string? HonorarioFator;
        public string? ValorPago;
    }
    public class ListaDados
    {
        public string? TipoGuia;
        public List<DadosFinanceiros>? Dados;

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
    // public string CopiarOuConverterParaXlsx(string origem, string destino)
    // {
    //     if (EhArquivoXlsx(origem))
    //     {
    //         // Se já for .xlsx, apenas copiamos
    //         File.Copy(origem, destino, overwrite: true);
    //         PathToFile = destino;
    //         return $"✅ Arquivo já era .xlsx. Cópia salva em: {destino}";
    //     }
    //     else
    //     {
    //         // Se for .xls, convertemos para .xlsx
    //         ConverterXlsParaXlsx(origem, destino);
    //         PathToFile = destino;
    //         return $"📄 Arquivo era .xls. Convertido e salvo como .xlsx em: {destino}";
    //     }
    // }    
    private bool EhArquivoXlsx(string caminho)
    {
        using var stream = new FileStream(caminho, FileMode.Open, FileAccess.Read);
        byte[] buffer = new byte[8];
        stream.Read(buffer, 0, 8);
        stream.Position = 0;

        // Assinatura binária de arquivos .xls
        long xlsSignature = unchecked((long)0xE11AB1A1E011CFD0);
        long fileSignature = BitConverter.ToInt64(buffer, 0);

        return fileSignature != xlsSignature; // Se for diferente de .xls, assumimos que é .xlsx
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
    // public void ConverterXlsParaXlsx(string caminhoXls, string caminhoXlsx)
    // {
    //     using var fileStream = new FileStream(caminhoXls, FileMode.Open, FileAccess.Read);
    //     var hssfWorkbook = new HSSFWorkbook(fileStream); // Lê .xls

    //     var xssfWorkbook = new XSSFWorkbook(); // Novo .xlsx

    //     for (int i = 0; i < hssfWorkbook.NumberOfSheets; i++)
    //     {
    //         var oldSheet = hssfWorkbook.GetSheetAt(i);
    //         var newSheet = xssfWorkbook.CreateSheet(oldSheet.SheetName);

    //         for (int row = 0; row <= oldSheet.LastRowNum; row++)
    //         {
    //             var oldRow = oldSheet.GetRow(row);
    //             if (oldRow == null) continue;

    //             var newRow = newSheet.CreateRow(row);
    //             for (int col = 0; col < oldRow.LastCellNum; col++)
    //             {
    //                 var oldCell = oldRow.GetCell(col);
    //                 if (oldCell != null)
    //                 {
    //                     var newCell = newRow.CreateCell(col);
    //                     newCell.SetCellValue(oldCell.ToString());
    //                 }
    //             }
    //         }
    //     }

    //     using var outFile = new FileStream(caminhoXlsx, FileMode.Create, FileAccess.Write);
    //     xssfWorkbook.Write(outFile);
    // }  
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
                dado.ValorTabela = row.Cell("I").GetString();
                dado.GrauParticipacao = row.Cell("X").GetString(); // GP
                dado.PercentualVia = row.Cell("Y").GetString();    // PV
                dado.HonorarioFator = row.Cell("Z").GetString();   // AHE
                dado.ValorPago = row.Cell("J").GetString();

                listaAtual.Dados?.Add(dado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar linha {row.RowNumber()}: {ex.Message}");
            }
        }

        return listaCompleta;
    }
    public string ObterDadosExtraidosComoJson(List<ListaDados> dadosExtraidos)
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