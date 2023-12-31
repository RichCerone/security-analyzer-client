﻿
using SecurityAnalyzer.DataModels.Google;
using SecurityAnalyzer.Helper;

namespace SecurityAnalyzer.DataModels.GitHub
{
    /// <summary>
    /// Represents a security analysis of a repository 
    /// in GitHub.
    /// </summary>
    public class GitHubSecurityAnalysis
    {
        /// <summary>
        /// Name of the repo being analyzed.
        /// </summary>
        public string RepoName { get; set; }

        /// <summary>
        /// The GitHub advisory information.
        /// </summary>
        public Advisory? Advisory { get; set; }

        /// <summary>
        /// The GitHub commit information.
        /// </summary>
        public GitHubCommit? GitHubCommit { get; set; }

        /// <summary>
        /// The Google Git commit information.
        /// </summary>
        public GoogleCommit? GoogleGitCommit { get; set; }

        /// <summary>
        /// The GitHub Pull Request information.
        /// </summary>
        public GitHubPullRequest? PullRequest { get; set; }

        /// <summary>
        /// The GitHub release information.
        /// </summary>
        public GitHubRelease? Release { get; set; }

        /// <summary>
        /// Any message to regarding this analysis.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The start of the exposure.
        /// </summary>
        public DateTime? StartOfExposure { get; set; }
        
        /// <summary>
        /// The end of the exposure.
        /// </summary>
        public DateTime? EndOfExposure { get; set; }

        /// <summary>
        /// Creates a new <see cref="GitHubSecurityAnalysis"/>.
        /// </summary>
        /// <param name="repoName">
        /// The name of the repo being analyzed.
        /// </param>
        /// <param name="advisory">
        /// The GitHub advisory data.
        /// </param>
        /// <param name="gitHubCommit">
        /// The GitHub commit data.
        /// </param>
        /// <param name="googleCommit">
        /// The Google Git commit data.
        /// </param>
        /// <param name="pullRequest">
        /// The GitHub pull request data.
        /// </param>
        /// <param name="release">
        /// The GitHub release data.
        /// </param>
        /// <param name="message">
        /// Message regarding this analysis.
        /// </param>
        public GitHubSecurityAnalysis(string repoName, Advisory? advisory, GitHubCommit? gitHubCommit, GoogleCommit? googleCommit, GitHubPullRequest? pullRequest, GitHubRelease? release, string message = "")
        {
            RepoName = repoName;
            Advisory = advisory;
            GitHubCommit = gitHubCommit;
            GoogleGitCommit = googleCommit;
            PullRequest = pullRequest;
            Release = release;
            Message= message;
        }
    }
}
