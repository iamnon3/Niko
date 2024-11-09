using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trak_IT.Scripts
{
    internal class SheetService
    {
        public async Task CopyJsonKeyToAppDataDirectoryAsync()
        {
            string fileName = "your-json file";
            string targetPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            if (!File.Exists(targetPath))
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
                using var reader = new StreamReader(stream);
                using var writer = File.CreateText(targetPath);

                await writer.WriteAsync(await reader.ReadToEndAsync());
            }
        }
        public SheetsService GetSheetsService()
        {
            var jsonPath = Path.Combine(FileSystem.AppDataDirectory, "your-json file");
            GoogleCredential credential;

            using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(SheetsService.Scope.Spreadsheets);
            }

            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "app name",
            });
        }
        public async Task<IList<IList<object>>> ReadSheetDataAsync(string spreadID, string spreadName)
        {
            var service = GetSheetsService();

            var request = service.Spreadsheets.Values.Get(spreadID, spreadName);
            var response = await request.ExecuteAsync();

            return response.Values;
        }

        public async Task PostStudent(string spreadsheetId, string range, IList<IList<object>> values)
        {
            var service = GetSheetsService();

            var valueRange = new Google.Apis.Sheets.v4.Data.ValueRange
            {
                Values = values
            };

            var updateRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            var response = await updateRequest.ExecuteAsync();
            Console.WriteLine("Data updated successfully.");
        }

        public async Task<string> FindAndUpdateRowAsync(string spreadsheetId, string searchValue, IList<object> updatedValues)
        {
            var service = GetSheetsService();

            // Read data from the sheet
            var readRequest = service.Spreadsheets.Values.Get(spreadsheetId, $"Sheet1!A:H");
            var readResponse = await readRequest.ExecuteAsync();
            var values = readResponse.Values;

            if (values == null || values.Count == 0)
                return null;

            // Locate the row to update
            int rowIndex = -1;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].Count > 0 && values[i][1].ToString() == searchValue)
                {
                    rowIndex = i;
                    break;
                }
            }

            if (rowIndex == -1)
                return null;

            // Construct the range for the specific row to update (e.g., "Sheet1!A{rowNumber}:C{rowNumber}")
            string updateRange = $"SHEET!G{rowIndex + 1}";

            var valueRange = new Google.Apis.Sheets.v4.Data.ValueRange
            {
                Values = new List<IList<object>> { updatedValues }
            };

            // Update the specific row
            var updateRequest = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, updateRange);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var updateResponse = await updateRequest.ExecuteAsync();

            Console.WriteLine("Row updated successfully.");
            return "Updated";
        }



    }
}
