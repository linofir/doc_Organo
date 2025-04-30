using DocAPI.Core.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using UglyToad.PdfPig.Fonts.Type1;

namespace DocAPI.Services
{
    public class GoogleSheetsDB
    {
        private readonly string[] Scopes = 
        { 
            SheetsService.Scope.Spreadsheets
        };
        private string _spreadsheetId = "1c-JIXHEGh8grMMBfL6ynUsTu7s-Lbw1ajD7bolFNpY8";
        private readonly string ApplicationName = "MinhaAppPlanilhas";

        private SheetsService CreateService()
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
             return service;
        }
        public void LerPlanilha()
        {
            var service = CreateService();

            // ID da planilha e range (ex: aba "Página1", colunas A até E)
            var range = "Pacientes!A1:E10";

            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(_spreadsheetId, range);
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
        public async Task<IList<IList<object>>> LerRangeAsync(string range)
        {
            var service = CreateService();

            var request = service.Spreadsheets.Values.Get(_spreadsheetId, range);
            var response = await request.ExecuteAsync();
            // Console.WriteLine(response.Values.Count);
            return response.Values;
        }
        public async Task WriteRangeAsync(string cell, IList<IList<object?>> value)
        {
            var service = CreateService();
            var valueRange = new ValueRange
            {
                Values = value
            };

            var updateRequest = service.Spreadsheets.Values.Update(valueRange, _spreadsheetId, cell);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            await updateRequest.ExecuteAsync();
        }

    }
}
