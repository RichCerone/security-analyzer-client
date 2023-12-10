
namespace SecurityAnalyzerGraph.DataModels
{
    /// <summary>
    /// Represents the repo and how many CVEs affected it.
    /// </summary>
    internal class MostAffectedRepo
    {
        /// <summary>
        /// The name of the repo affected.
        /// </summary>
        public string RepoName { get; set; } = string.Empty;

        /// <summary>
        /// The number of CVE's that affected this repo.
        /// </summary>
        public double NumberOfCves { get; set; }
    }
}
