using Newtonsoft.Json;
using SecurityAnalyzer.DataModels.GitHub;

namespace SecurityAnalyzerGraph.Services
{
    /// <summary>
    /// Helps read the analysis report built by the SecurityAnalzyer project.
    /// </summary>
    internal static class ReportReader
    {
        /// <summary>
        /// Reads the analysis report for analyses.
        /// </summary>
        /// <param name="path">
        /// The aboslute path to the report JSON.
        /// </param>
        /// <returns>
        /// <see cref="IEnumerable{T}"/> of <see cref="GitHubSecurityAnalysis"/> generated
        /// by the SecurityAnalzyer project.
        /// </returns>
        /// <exception cref="JsonSerializationException">
        /// Thrown if the file cannot be deserialzed into a 
        /// <see cref="IEnumerable{T}"/> of <see cref="GitHubSecurityAnalysis"/>.
        /// </exception>
        public static async Task<IEnumerable<GitHubSecurityAnalysis>> ReadAnalysesAsync(string path)
        {
            string json = await File.ReadAllTextAsync(path);
            IEnumerable<GitHubSecurityAnalysis> analyses = JsonConvert.DeserializeObject<IEnumerable<GitHubSecurityAnalysis>>(json)
                ?? throw new JsonSerializationException("Could not deserialzie security spec json.");

            return analyses;
        }
    }
}
