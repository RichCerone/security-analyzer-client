using Newtonsoft.Json;
using SecurityAnalyzer.DataModels.GitHub;

namespace SecurityAnalyzer.Services
{
    internal class ReportService
    {
        public static async Task GenerateReportAsync(IEnumerable<GitHubSecurityAnalysis> analyses)
        {
            try
            {
                string text = JsonConvert.SerializeObject(analyses, Formatting.Indented);

                await File.WriteAllTextAsync($"../../../Reports/{DateTime.Now:yyyy-MM-dd HH-mm-ss}-report.json", text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GenerateReport: {ex}");
            }
        }
    }
}
