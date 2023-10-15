using Newtonsoft.Json;
using SecurityAnalyzer.DataModels.GitHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAnalyzerGraph.Services
{
    internal static class ReportReader
    {
        public static async Task<IEnumerable<GitHubSecurityAnalysis>> ReadAnalysesAsync()
        {
            string json = await File.ReadAllTextAsync(@"C:\Workspace\CS700B\security-analyzer-client\SecurityAnalyzerGraph\Reports\2023-10-12 13-52-37-report.json");
            IEnumerable<GitHubSecurityAnalysis> analyses = JsonConvert.DeserializeObject<IEnumerable<GitHubSecurityAnalysis>>(json)
                ?? throw new JsonSerializationException("Could not deserialzie security spec json.");

            return analyses;
        }
    }
}
