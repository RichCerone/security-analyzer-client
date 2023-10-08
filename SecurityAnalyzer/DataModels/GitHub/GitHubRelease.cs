using Newtonsoft.Json;

namespace SecurityAnalyzer.DataModels.GitHub
{
    /// <summary>
    /// Represents a release on GitHub.
    /// </summary>
    internal class GitHubRelease
    {
        [JsonProperty("message")]
        public ReleaseMessage? Message { get; set; }
    }

    internal class ReleaseMessage
    {
        [JsonProperty("url")]
        public string ReleaseUrl { get; set; } = string.Empty;

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
