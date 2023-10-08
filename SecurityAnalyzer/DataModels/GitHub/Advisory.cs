using Newtonsoft.Json;

namespace SecurityAnalyzer.DataModels.GitHub
{
    /// <summary>
    /// Represents a GitHub Security Advisory.
    /// </summary>
    internal class Advisory
    {
        /// <summary>
        /// The GitHub Security Advisory id.
        /// </summary>
        [JsonProperty("ghsa_id")]
        public string GhsaId { get; set; } = string.Empty;

        /// <summary>
        /// The CVE Number.
        /// </summary>
        [JsonProperty("cve_id")]
        public string CveId { get; set; } = string.Empty;

        /// <summary>
        /// The URL to the GitHub Security Advisory.
        /// </summary>
        [JsonProperty("html_url")]
        public string AdvisoryUrl { get; set; } = string.Empty;

        /// <summary>
        /// A short summary of the security advisory. 
        /// </summary>
        [JsonProperty("summary")]
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// URLs to references where this security advisory was sourced.
        /// </summary>
        [JsonProperty("references")]
        public IEnumerable<string>? References { get; set; }

        /// <summary>
        /// Date and time GitHub published this security advisory.
        /// </summary>
        [JsonProperty("published_at")]
        public DateTime? PublishedAt { get; set; }

        /// <summary>
        /// Date and time the NVD published this security advisory.
        /// </summary>
        [JsonProperty("nvd_published_at")]
        public DateTime? NvdePublishedAt { get; set; }

        /// <summary>
        /// Vulnerable versions of this repository.
        /// </summary>
        [JsonProperty("vulnerabilities")]
        public IEnumerable<Vulnerability>? Vulnerabilities { get; set; }
    }

    /// <summary>
    /// Represents the collection of GitHub Security Advisories.
    /// </summary>
    internal class AdvisoryMessages
    {
        [JsonProperty("message")]
        public IEnumerable<Advisory>? Advisories { get; set; }
    }
}
