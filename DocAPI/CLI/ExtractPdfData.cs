using DocAPI.Services;

namespace DocAPI.CLI
{
    public class ExtractExamesCli
    {
        public static void Run(string[] args)
        {
            Console.WriteLine("Iniciando extração dos dados do PDF...");

            try
            {
                PdfExtractorTabela.ExtrairExamesComCodigo();
                Console.WriteLine("Extração concluída com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na extração: {ex.Message}");
            }
        }
    }
}
