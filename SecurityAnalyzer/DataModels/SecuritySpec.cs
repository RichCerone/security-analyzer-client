
namespace SecurityAnalyzer.DataModels
{
    /// <summary>
    /// Represents the specification of the security analysis
    /// we want to peform.
    /// </summary>
    internal class SecuritySpec
    {
        /// <summary>
        /// The owner of the repo.
        /// </summary>
        public string RepoOwner { get; set; }

        /// <summary>
        /// Name of the repo to analyze.
        /// </summary>
        public string RepoName { get; set; }

        /// <summary>
        /// A list of CVE's related to this repo.
        /// </summary>
        public IEnumerable<string> Cves { get; set;}

        /// <summary>
        /// Any prefix to pre-append to the version tag.
        /// </summary>
        public string VersionTagPerfix { get; set; }

        /// <summary>
        /// Creates a new <see cref="SecuritySpec"/>.
        /// </summary>
        /// <param name="repoOwner">
        /// The owner of the repo.
        /// </param>
        /// <param name="repoName">
        /// The repo to analzye.
        /// </param>
        /// <param name="cves">
        /// <see cref="IEnumerable{T}"/> of <see cref="string"/>
        /// representing CVE error codes.
        /// </param>
        /// <param name="versionTagPrefx">
        /// The prefix to pre-appeend to the version tag (blank by default).
        /// </param>
        public SecuritySpec(string repoOwner, string repoName, IEnumerable<string> cves, string versionTagPrefx = "") 
        {
            RepoOwner = repoOwner;
            RepoName = repoName;
            Cves = cves;
            VersionTagPerfix = versionTagPrefx;
        }
    }
}
