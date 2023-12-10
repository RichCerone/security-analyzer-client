
namespace SecurityAnalyzerGraph.DataModels
{
    /// <summary>
    /// Represents the CVE and how many repos it affected.
    /// </summary>
    internal class MostAffectiveCve
    {
        /// <summary>
        /// The CVE.
        /// </summary>
        public string Cve {  get; set; } = string.Empty;

        /// <summary>
        /// The number of repos the CVE affected.
        /// </summary>
        public double NumberOfRepos { get; set; }
    }
}
