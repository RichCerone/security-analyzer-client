using Newtonsoft.Json;
using SecurityAnalyzer.DataModels.GitHub;

namespace SecurityAnalyzer.Services
{
    /// <summary>
    /// Helps write the report output.
    /// </summary>
    internal class ReportService
    {
        /// <summary>
        /// Generates a JSON report for the final analyses.
        /// </summary>
        /// <param name="analyses">
        /// <see cref="IEnumerable{T}"/> of <see cref="GitHubSecurityAnalysis"/>
        /// </param>
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
