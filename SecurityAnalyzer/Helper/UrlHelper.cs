using SecurityAnalyzer.DataModels;
using System.Text.RegularExpressions;

namespace SecurityAnalyzer.Helper
{
    /// <summary>
    /// Helper functions for analyzing URLs.
    /// </summary>
    internal static class UrlHelper
    {
        /// <summary>
        /// Analyzes each url for a url that matches a commit.
        /// </summary>
        /// <param name="urls">
        /// The URLs to analyze.
        /// </param>
        /// <returns>
        /// <see cref="IEnumerable{T}"/> of <see cref="CommitHash" />
        /// </returns>
        public static IEnumerable<CommitHash> AnalyzeForCommitUrls(IEnumerable<string> urls)
        {
            List<CommitHash> commitHashes = new();

            // Use regex groups so we can extract the commit hash if present.
            string gitHubCommitPattern = @"https:\/\/github\.com\/([A-Za-z0-9-]+)\/([A-Za-z0-9-]+)\/commit\/([A-Fa-f0-9]+)";
            Regex regex = new (gitHubCommitPattern, RegexOptions.Compiled, TimeSpan.FromSeconds(90));

            foreach (string url in urls)
            {
                /* If the url matches and we have 4 groupings, 
                 * then the commit hash is present to use.
                 */
                Match match = regex.Match(url);
                if (match.Success && match.Groups.Count == 4)
                {
                    CommitHash commitHash = new(match.Groups[3].Value, match.Groups[2].Value, match.Groups[1].Value);
                    commitHashes.Add(commitHash);
                }
            }

            return commitHashes;
        }

        /// <summary>
        /// Analyzes each url for a url that matches a pull request.
        /// </summary>
        /// <param name="urls">
        /// The URLs to analyze.
        /// </param>
        /// <returns>
        /// <see cref="IEnumerable{T}"/> of <see cref="PullRequest"/> pull request ids
        /// </returns>
        public static IEnumerable<PullRequest> AnalyzeForPullUrls(IEnumerable<string> urls)
        {
            List<PullRequest> pullNumbers = new();

            // Use regex groups so we can extract pull request number if present.
            string gitHubPullPattern = @"https:\/\/github\.com\/([\w-]+)\/([\w-]+)\/pull\/(\d+)";
            Regex regex = new(gitHubPullPattern, RegexOptions.Compiled, TimeSpan.FromSeconds(90));

            foreach (string url in urls)
            {
                /* If the url matches and we have 4 groupings, 
                 * then the pull hash is present to use.
                 */
                Match match = regex.Match(url);
                if (match.Success && match.Groups.Count == 4)
                {
                    pullNumbers.Add(new PullRequest(int.Parse(match.Groups[3].Value), match.Groups[1].Value, match.Groups[2].Value));
                }
            }

            return pullNumbers;
        }


        public static IEnumerable<string> AnalyzeForGoogleGitCommits(IEnumerable<string> urls)
        {
            List<string> commitUrls = new();

            string googleGitUrlPattern = "https://android\\.googlesource\\.com/platform/[A-Za-z]+/[A-Za-z]+/[A-Za-z]+/\\+/[A-Za-z0-9]+";
            Regex regex = new(googleGitUrlPattern, RegexOptions.Compiled, TimeSpan.FromSeconds(90));

            foreach (string url in urls)
            {
                Match match = regex.Match(url);
                if (match.Success)
                {
                    commitUrls.Add(url);
                }
            }

            return commitUrls;
        }
    }
}
