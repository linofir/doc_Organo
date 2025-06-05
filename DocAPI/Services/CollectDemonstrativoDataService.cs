using ClosedXML;
using NPOI.HSSF.UserModel;  // Para .xls
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // Para .xlsx
using System.IO;

namespace DocAPI.Services;
public class CollectDemonstrativoDataService
{
    public CollectDemonstrativoDataService(string caminhoXls)
    {
        string novoCaminho = @"C:\Users\lino\Dropbox\io\Doc_Organo\Descricao.xlsx";
        string resultado = CopiarOuConverterParaXlsx(caminhoXls, novoCaminho);
        Console.WriteLine(resultado);
    }
    private string PathToFile;
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
    public string CopiarOuConverterParaXlsx(string origem, string destino)
    {
        if (EhArquivoXlsx(origem))
        {
            // Se já for .xlsx, apenas copiamos
            File.Copy(origem, destino, overwrite: true);
            PathToFile = destino;
            return $"✅ Arquivo já era .xlsx. Cópia salva em: {destino}";
        }
        else
        {
            // Se for .xls, convertemos para .xlsx
            ConverterXlsParaXlsx(origem, destino);
            PathToFile = destino;
            return $"📄 Arquivo era .xls. Convertido e salvo como .xlsx em: {destino}";
        }
    }    
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
        var hssfWorkbook = new HSSFWorkbook(fileStream); // Lê .xls

        var xssfWorkbook = new XSSFWorkbook(); // Novo .xlsx

        for (int i = 0; i < hssfWorkbook.NumberOfSheets; i++)
        {
            var oldSheet = hssfWorkbook.GetSheetAt(i);
            var newSheet = xssfWorkbook.CreateSheet(oldSheet.SheetName);

            for (int row = 0; row <= oldSheet.LastRowNum; row++)
            {
                var oldRow = oldSheet.GetRow(row);
                if (oldRow == null) continue;

                var newRow = newSheet.CreateRow(row);
                for (int col = 0; col < oldRow.LastCellNum; col++)
                {
                    var oldCell = oldRow.GetCell(col);
                    if (oldCell != null)
                    {
                        var newCell = newRow.CreateCell(col);
                        newCell.SetCellValue(oldCell.ToString());
                    }
                }
            }
        }

        using var outFile = new FileStream(caminhoXlsx, FileMode.Create, FileAccess.Write);
        xssfWorkbook.Write(outFile);
    }  
    
}