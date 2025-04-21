using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;

namespace DocAPI.Services
{
    public class GoogleSheetsDB
    {
        private readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private readonly string ApplicationName = "MinhaAppPlanilhas";

        public void LerPlanilha()
        {
            // Caminho da chave JSON
            var credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "Secrets", "personalcloud-405819-0fa9be3ab71c.json");

            GoogleCredential credential;
            using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            // Inicializar serviço da API
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // ID da planilha e range (ex: aba "Página1", colunas A até E)
            var spreadsheetId = "1c-JIXHEGh8grMMBfL6ynUsTu7s-Lbw1ajD7bolFNpY8";
            var range = "Pacientes!A1:E10";

            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            var values = response.Values;

            if (values == null || values.Count == 0)
            {
                Console.WriteLine("Nenhum dado encontrado.");
            }
            else
            {
                foreach (var row in values)
                {
                    Console.WriteLine(string.Join(" | ", row));
                }
            }
        }
    }
}
