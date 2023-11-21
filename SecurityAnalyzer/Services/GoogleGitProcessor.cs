using Microsoft.Extensions.Configuration;
using SecurityAnalyzer.DataModels.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SecurityAnalyzer.Services
{
    /// <summary>
    /// Processes the Google Git site for commit data.
    /// </summary>
    internal class GoogleGitProcessor
    {
        /// <summary>
        /// Creates a new <see cref="GoogleGitProcessor"/>.
        /// </summary>
        public GoogleGitProcessor() 
        {        
        }

        /// <summary>
        /// Gets a Google commit using the commit URL.
        /// </summary>
        /// <param name="commitUrl">
        /// The URL leading to the commit page on Google Git.
        /// </param>
        /// <returns>
        /// <see cref="GoogleCommit"/>
        /// </returns>
        public async Task<GoogleCommit?> GetGoogleCommitAsync(string commitUrl)
        {
            GoogleCommit? commit = new();

            try
            {
                using HttpClient httpClient = new();
                HttpResponseMessage response = await httpClient.GetAsync(commitUrl);
                response.EnsureSuccessStatusCode();

                string html = await response.Content.ReadAsStringAsync();

                commit.Comitter = GetCommitAuthorFromHtml(html);
                commit.DateTime = GetCommitDateFromHtml(html);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"GetGoogleCommitAsync HTTP Error: {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetGoogleCommitAsync Error: {ex}");
                throw;
            }

            if (commit.Comitter == string.Empty &&  commit.DateTime == null)
            {
                commit = null;
            }

            return commit;
        }

        /// <summary>
        /// Gets the commit author out of the HTML page of the
        /// Google Git commit.
        /// </summary>
        /// <param name="html">
        /// The HTML page to process.
        /// </param>
        /// <returns>
        /// The commit author.
        /// </returns>
        private static string GetCommitAuthorFromHtml(string html)
        {
            string comitter = string.Empty;

            // Use regex groups to get both the author's full name and email.
            string committerPattern = @"<th class=""Metadata-title"">committer</th><td>(.*?)&lt;(.*?)&gt;</td><td>(.*?)</td>";
            Regex regex = new(committerPattern);

            Match match = regex.Match(html);
            if (match.Success && match.Groups.Count > 2)
            {
                comitter += match.Groups[1];
                comitter += $" {match.Groups[2]}";
            }

            return comitter;
        }

        /// <summary>
        /// Gets the commit date from the HTML page of the
        /// Google Git commit.
        /// </summary>
        /// <param name="html">
        /// The HTML page to process.
        /// </param>
        /// <returns>
        /// The commit date as a <see cref="DateTime"/>.
        /// </returns>
        private static DateTime? GetCommitDateFromHtml(string html)
        {
            DateTime? commitDate = null;

            // Use regex groups to get both the author's full name and email.
            string committerPattern = @"<th class=""Metadata-title"">committer</th><td>.*?</td><td>(.*?)</td>";
            Regex regex = new(committerPattern);

            Match match = regex.Match(html);
            if (match.Success && match.Groups.Count > 1)
            {
                commitDate = DateTime.ParseExact(match.Groups[1].ToString(), "ddd MMM dd HH:mm:ss yyyy zzzz", System.Globalization.CultureInfo.InvariantCulture);
            }

            return commitDate;
        }
    }
}
