using SecurityAnalyzer.DataModels.GitHub;
using SecurityAnalyzerGraph.DataModels;
using SecurityAnalyzerGraph.Helpers;

namespace SecurityAnalyzerGraph.Services
{
    /// <summary>
    /// Generates coordinates to use for graphing.
    /// </summary>
    internal class CoordinateGenerator
    {
        /// <summary>
        /// Generates a graph showing the median time in days it took to close/patch 
        /// a CVE on an affected repo.
        /// </summary>
        /// <param name="analyses">
        /// <see cref="IEnumerable{T}"/> of <see cref="GitHubSecurityAnalysis"/>
        /// </param>
        /// <param name="ascending">
        /// Whther the order of the graph should be in ascending or descending order
        /// by the median closure time (days).
        /// </param>
        /// <returns>
        /// <see cref="List{T}"/> of <see cref="AverageClosure"/>
        /// </returns>
        public static List<AverageClosure> GenerateMedianClosurePerCve(IEnumerable<GitHubSecurityAnalysis> analyses, bool ascending)
        {
            List<AverageClosure> coordinates = new();
            foreach (GitHubSecurityAnalysis analysis in analyses)
            {
                if (analysis.Advisory?.CveId == null)
                {
                    continue;
                }

                int timeToClose = 0;
                if (analysis.StartOfExposure.HasValue && analysis.EndOfExposure.HasValue)
                {
                    timeToClose = (analysis.StartOfExposure - analysis.EndOfExposure).Value.Days;

                    AverageClosure coordinate = new()
                    {
                        Cve = analysis.Advisory.CveId,
                        TimeToClose = timeToClose,
                        AffectedRepos = new List<string>() { analysis.RepoName }
                    };

                    coordinates.Add(coordinate);
                }
            }

            IEnumerable<IGrouping<string, AverageClosure>> repoWithCves = coordinates.GroupBy(c => c.Cve);
            List<AverageClosure> newCoordinates = new();
            foreach (IGrouping<string, AverageClosure> cve in repoWithCves)
            {
                newCoordinates.Add(new AverageClosure()
                {
                    Cve = cve.Key,
                    TimeToClose = Statistics.GetMedian(cve.Select(c => c.TimeToClose)),
                    AffectedRepos = cve.SelectMany(c => c.AffectedRepos)
                });
            }

            if (ascending) return newCoordinates.OrderBy(c => c.TimeToClose).ToList();
            else return newCoordinates.OrderByDescending(c => c.TimeToClose).ToList();
        }

        /// <summary>
        /// Generates a graph showing repos affected by the number of CVEs.
        /// </summary>
        /// <param name="analyses">
        /// <see cref="IEnumerable{T}"/> of <see cref="GitHubSecurityAnalysis"/>
        /// </param>
        /// <param name="ascending">
        /// Whther the order of the graph should be in ascending or descending order by number of CVE's.
        /// </param>
        /// <returns>
        /// <see cref="List{T}"/> of <see cref="MostAffectiveCve"/>
        /// </returns>
        public static List<MostAffectedRepo> GenerateMostAffectedRepos(IEnumerable<GitHubSecurityAnalysis> analyses, bool ascending)
        {
            List<MostAffectedRepo> coordinates = new();
            Dictionary<string, int> repoToCve = new(); 
            foreach (GitHubSecurityAnalysis analysis in analyses)
            {
                if (analysis.Advisory?.CveId == null)
                {
                    continue;
                }

                if (!repoToCve.ContainsKey(analysis.RepoName))
                {
                    repoToCve.Add(analysis.RepoName, 1);
                }
                else
                {
                    repoToCve[analysis.RepoName]++;
                }
            }

            foreach (KeyValuePair<string, int> pair in repoToCve)
            {
                coordinates.Add(new MostAffectedRepo()
                {
                    RepoName = pair.Key,
                    NumberOfCves = pair.Value,
                });
            }

            if (ascending) return coordinates.OrderBy(c => c.NumberOfCves).ToList();
            else return coordinates.OrderByDescending(c => c.NumberOfCves).ToList();
        }

        /// <summary>
        /// Generates a graph showing CVE's affecting the number of repos.
        /// </summary>
        /// <param name="analyses">
        /// <see cref="IEnumerable{T}"/> of <see cref="GitHubSecurityAnalysis"/>
        /// </param>
        /// <param name="ascending">
        /// Whther the order of the graph should be in ascending or descending order by number of repos.
        /// </param>
        /// <returns>
        /// <see cref="List{T}"/> of <see cref="MostAffectiveCve"/>
        /// </returns>
        public static List<MostAffectiveCve> GenerateMostAffectiveCves(IEnumerable<GitHubSecurityAnalysis> analyses, bool ascending)
        {
            List<MostAffectiveCve> coordinates = new();
            Dictionary<string, int> cveToRepo = new();
            foreach (GitHubSecurityAnalysis analysis in analyses)
            {
                if (analysis.Advisory?.CveId == null)
                {
                    continue;
                }

                if (!cveToRepo.ContainsKey(analysis.Advisory.CveId))
                {
                    cveToRepo.Add(analysis.Advisory.CveId, 1);
                }
                else
                {
                    cveToRepo[analysis.Advisory.CveId]++;
                }
            }

            foreach (KeyValuePair<string, int> pair in cveToRepo)
            {
                coordinates.Add(new MostAffectiveCve()
                {
                    Cve = pair.Key,
                    NumberOfRepos = pair.Value,
                });
            }

            if (ascending) return coordinates.OrderBy(c => c.NumberOfRepos).ToList();
            else return coordinates.OrderByDescending(c => c.NumberOfRepos).ToList();
        }
    }
}
