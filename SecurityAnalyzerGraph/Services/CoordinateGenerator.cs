using SecurityAnalyzer.DataModels.GitHub;
using SecurityAnalyzerGraph.DataModels;

namespace SecurityAnalyzerGraph.Services
{
    internal class CoordinateGenerator
    {
        public static List<ClosureResponse> GenerateClosureResponseForCve(IEnumerable<GitHubSecurityAnalysis> analyses)
        {
            List<ClosureResponse> coordinates = new();

            foreach (GitHubSecurityAnalysis analysis in analyses)
            {
                int timeToClose = 0;
                if (analysis.StartOfExposure.HasValue && analysis.EndOfExposure.HasValue)
                {
                    timeToClose = (analysis.StartOfExposure - analysis.EndOfExposure).Value.Days;
                }

                if (analysis.Advisory != null && !string.IsNullOrWhiteSpace(analysis.Advisory.CveId))
                {
                    ClosureResponse coordinate = new()
                    {
                        RepoName = analysis.RepoName,
                        Cve = analysis.Advisory?.CveId ?? string.Empty,
                        TimeToClose = timeToClose
                    };

                    coordinates.Add(coordinate);
                }
            }

            return coordinates;
        }
    }
}
