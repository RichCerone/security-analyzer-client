
namespace SecurityAnalyzerGraph.DataModels
{
    /// <summary>
    /// Holds details on when a CVE was closed
    /// for a repo.
    /// </summary>
    internal class ClosureResponse
    {
        /// <summary>
        /// The name of the repo.
        /// </summary>
        public string RepoName { get; set; } = string.Empty;

        /// <summary>
        /// The CVE where the vulnerability was closed.
        /// </summary>
        public string Cve { get; set; } = string.Empty;

        /// <summary>
        /// The time it took to close in days.
        /// </summary>
        public double TimeToClose { get; set; }
    }
}
