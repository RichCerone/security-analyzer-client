
namespace SecurityAnalyzerGraph.DataModels
{
    /// <summary>
    /// Repersents the average time it took to close CVE vulnerabilities.
    /// </summary>
    internal class AverageClosure
    {
        /// <summary>
        /// The CVE to see if it was closed.
        /// </summary>
        public string Cve { get; set; } = string.Empty;

        /// <summary>
        /// The average time in days to close the CVE.
        /// </summary>
        public double TimeToClose { get; set; }

        /// <summary>
        /// The repositories affected by this CVE.
        /// </summary>
        public IEnumerable<string> AffectedRepos { get; set; } = new List<string>();
    }
}
