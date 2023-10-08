﻿using Newtonsoft.Json;

namespace SecurityAnalyzer.DataModels.GitHub
{
    /// <summary>
    /// Reperesents a pull request in GitHub.
    /// </summary>
    internal class GitHubPullRequest
    {
        [JsonProperty("message")]
        public PullRequestMessage? Message { get; set; }
    }

    internal class PullRequestMessage
    {
        /// <summary>
        /// The date and time this pull request was completed and merged.
        /// </summary>
        [JsonProperty("merged_at")]
        public DateTime? MergedAt { get; set; }
    }
}
