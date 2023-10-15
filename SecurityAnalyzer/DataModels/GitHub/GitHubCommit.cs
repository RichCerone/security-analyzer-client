using Newtonsoft.Json;

namespace SecurityAnalyzer.DataModels.GitHub
{
    /// <summary>
    /// Represents a GitHub commit.
    /// </summary>
    public class GitHubCommit
    {
        [JsonProperty("message")]
        public CommitMessage? Message { get; set; }
    }

    /// <summary>
    /// Represents the message in the commit.
    /// </summary>
    public class CommitMessage
    {
        [JsonProperty("commit")]
        public Commit? Commit { get; set; }
    }

    /// <summary>
    /// Represents a commit.
    /// </summary>
    public class Commit
    {
        [JsonProperty("author")]
        public Author? Author { get; set; }
    }

    /// <summary>
    /// Represents the commit author.
    /// </summary>
    public class Author
    {
        [JsonProperty ("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("date")]
        public DateTime? Date {  get; set; }
    }
}
