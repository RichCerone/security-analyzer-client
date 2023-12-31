﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Polly;
using SecurityAnalyzer.DataModels;
using SecurityAnalyzer.DataModels.GitHub;
using SecurityAnalyzer.DataModels.Google;
using SecurityAnalyzer.Exceptions;
using SecurityAnalyzer.Helper;
using System.Collections.Specialized;
using System.Data;
using System.Net;
using System.Web;

namespace SecurityAnalyzer.Services
{
    /// <summary>
    /// Processes the GitHub API to generate a
    /// security analysis.
    /// </summary>
    internal class GitHubProcessor
    {
        private readonly IConfiguration _config;
        private readonly GoogleGitProcessor _googleGitProcessor;
        private readonly HttpClient httpClient;

        /// <summary>
        /// Creates a new <see cref="GitHubProcessor"/>.
        /// </summary>
        /// <param name="config">
        /// <see cref="IConfiguration"/> that contains configuration settings.
        /// </param>
        /// <param name="googleGitProcessor">
        /// Process Google Git site data.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if the GitHubEndpoint is undefined in the <see cref="IConfiguration"/>.
        /// </exception>
        public GitHubProcessor(IConfiguration config, GoogleGitProcessor googleGitProcessor)
        {
            _config = config;
            _googleGitProcessor = googleGitProcessor;

            httpClient = new HttpClient()
            {
                BaseAddress = new Uri(_config["GitHubEndPoint"] 
                ?? throw new ArgumentException("GitHubEndPoint is not defined in the configuration."))
            };
        }

        /// <summary>
        /// Gets data to analyze from GitHub.
        /// </summary>
        /// <returns>
        /// <see cref="IEnumerable{T}"/> of <see cref="GitHubSecurityAnalysis"/>
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the user defined <see cref="IEnumerable{T}"/> of
        /// <see cref="SecuritySpec"/> is not defined or invalid.
        /// </exception>
        public async Task<IEnumerable<GitHubSecurityAnalysis>> GetSecurityAnalysisAsync()
        {
            List<GitHubSecurityAnalysis> analyses = new();
            try
            {
                IEnumerable<SecuritySpec> specs = await ReadSecuritySpecs();
                if (!specs.Any())
                {
                    throw new InvalidOperationException("No security specs defined.");
                }

                ResiliencePipeline retryPolicy = new ResiliencePipelineBuilder()
               .AddRetry(new Polly.Retry.RetryStrategyOptions()
               {
                   ShouldHandle = new PredicateBuilder().Handle<RateLimitException>(),
                   MaxRetryAttempts = 3,
                   Delay = TimeSpan.FromSeconds(3900) // Wait 65 minutes.
               })
               .Build();

                // Start foreach on repo and foreach on CVEs.
                foreach (SecuritySpec spec in specs)
                {
                    foreach (string cve in spec.Cves)
                    {                       
                        GitHubSecurityAnalysis analysis = await retryPolicy.ExecuteAsync(async (token) => await ProcessAdvisoryAsync(spec, cve).ConfigureAwait(false))
                            .ConfigureAwait(false);

                       analyses.Add(analysis);
                    }
                }

                return analyses;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"PerformSecurityAnalysisAsync Error: {ex}");
            }
            
            return analyses;
        }

        /// <summary>
        /// Processes the GitHub security advisory.
        /// </summary>
        /// <param name="spec">
        /// <see cref="SecuritySpec"/> to process.
        /// </param>
        /// <param name="cve">
        /// The CVE id to process.
        /// </param>
        /// <returns>
        /// <see cref="GitHubSecurityAnalysis"/>
        /// </returns>
        private async Task<GitHubSecurityAnalysis> ProcessAdvisoryAsync(SecuritySpec spec, string cve)
        {
            // Get initial advisory.
            Advisory? advisory = await GetAdvisoryAsync(spec.RepoName, cve);
            GitHubSecurityAnalysis analysis = new(spec.RepoName, advisory, null, null, null, null);

            if (advisory == null)
            {
                string message = $"No advisories could be retrieved using repo: '{spec.RepoName}' and CVE: '{cve}'";
                analysis.Message = message;

                return analysis;
            }

            // For each advisory, look for valid GitHub commit urls.
            if (advisory.References != null)
            {
                List<CommitHash> commitHashes = SearchReferencesForCommits(advisory.References).ToList();
                List<string> googleGitUrls = SearchReferencesForGoogleGitCommits(advisory.References).ToList();

                // If commit urls exist, GET commits and look for commit date.
                if (commitHashes.Any())
                {
                    List<GitHubCommit> commits = new();
                    foreach (CommitHash commitHash in commitHashes)
                    {
                        GitHubCommit? commit = await GetCommitAsync(commitHash.RepoOwner, commitHash.Repo, commitHash.Hash);
                        if (commit != null && commit.Message != null && commit.Message.Commit != null)
                        {
                            commits.Add(commit);
                        }
                    }

                    GitHubCommit? gitHubCommit = commits.OrderByDescending(c => c?.Message?.Commit?.Author?.Date).FirstOrDefault();
                    analysis.GitHubCommit = gitHubCommit;

                    if (gitHubCommit != null && gitHubCommit.Message != null && gitHubCommit.Message.Commit.Author.Date.HasValue)
                    {
                        return analysis;
                    }
                }
                else if (googleGitUrls.Any()) // If Google git URLs exist, get commit data.
                {
                    List<GoogleCommit?> commits = new();
                    foreach (string googleCommitUrl in googleGitUrls)
                    {
                        GoogleCommit? commit = await _googleGitProcessor.GetGoogleCommitAsync(googleCommitUrl);
                        if (commit != null)
                        {
                            commits.Add(commit);
                        }
                    }

                    GoogleCommit? googleCommit = commits.OrderByDescending(c => c?.DateTime).FirstOrDefault();
                    analysis.GoogleGitCommit = googleCommit;

                    if (googleCommit != null)
                    {
                        return analysis;
                    }
                }

                // For each advisory, look for valid GitHub Pull Request urls.
                List<PullRequest> pullRequests = SearchReferencesForPullRequests(advisory.References).ToList();
                if (pullRequests.Any())
                {
                    List<GitHubPullRequest> gitHubPullRequests = new();
                    foreach (PullRequest pull in pullRequests)
                    {
                        GitHubPullRequest? pullRequest = await GetPullRequestAsync(pull.RepoOwner, pull.Repo, pull.Id);
                        if (pullRequest != null && pullRequest.Message != null && pullRequest.Message.MergedAt != null)
                        {
                            gitHubPullRequests.Add(pullRequest);
                        }
                    }

                    GitHubPullRequest? gitHubPullRequest = gitHubPullRequests.OrderByDescending(p => p.Message?.MergedAt).FirstOrDefault();
                    analysis.PullRequest = gitHubPullRequest;

                    if (gitHubPullRequest != null && gitHubPullRequest.Message != null && gitHubPullRequest.Message.MergedAt.HasValue)
                    {
                        return analysis;
                    }
                }
            }

            // If no commits, look for releases by patched tags starting from earliest release.
            if (advisory.Vulnerabilities != null)
            {
                List<GitHubRelease> releases = new();
                foreach (Vulnerability vulnerability in advisory.Vulnerabilities)
                {
                    // Guess the release tag.
                    string tag = $"{spec.VersionTagPerfix}{vulnerability.FirstPatchedVersion}";
                    GitHubRelease? release = await GetRelaseAsync(spec.RepoOwner, spec.RepoName, tag);

                    if (release != null && release.Message != null && release.Message.CreatedAt.HasValue)
                    {
                        releases.Add(release);
                    }
                }

                if (releases.Any())
                {
                    GitHubRelease? release = releases.OrderByDescending(r => r.Message?.CreatedAt).FirstOrDefault();
                    if (release != null)
                    {
                        analysis.Release = release;
                    }
                }
            }

            return analysis;
        }

        /// <summary>
        /// Reads the security specs defined in the program configuration JSON.
        /// </summary>
        /// <returns>
        /// <see cref="IEnumerable{T}"/> of <see cref="SecuritySpec"/> defining
        /// what should be analyzed.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the configuration JSON contains an undefined security specs path.
        /// </exception>
        /// <exception cref="JsonSerializationException">
        /// Thrown if the security specs JSON defined in the configuration is not
        /// deserializable.
        /// </exception>
        private async Task<IEnumerable<SecuritySpec>> ReadSecuritySpecs()
        {
            List<SecuritySpec> specs = new();

            try
            {
                string json = await File.ReadAllTextAsync(_config["SecuritySpecsPath"] ??
                    throw new ArgumentException("SecuritySpecsPath: is not defined in the configuration"));

                Configuration config = JsonConvert.DeserializeObject<Configuration>(json) ?? 
                    throw new JsonSerializationException("Could not deserialzie security spec json.");

                if (config.SecuritySpecs != null)
                {
                    specs = config.SecuritySpecs.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ReadSecuritySpecs Error: {ex}");
                throw;
            }

            return specs;
        }

        /// <summary>
        /// Searches the references results for possible commits.
        /// </summary>
        /// <param name="references">
        /// <see cref="IEnumerable{T}"/> of <see cref="string"/> URL reference
        /// related to the advisory.
        /// </param>
        /// <returns>
        /// <see cref="IEnumerable{T}"/> of <see cref="CommitHash"/>
        /// </returns>
        private static IEnumerable<CommitHash> SearchReferencesForCommits(IEnumerable<string> references)
        {
            List<CommitHash> results = new();

            try
            {
                results.AddRange(UrlHelper.AnalyzeForCommitUrls(references));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SearchReferencesForCommits Error: {ex}");
                throw;
            }

            return results.DistinctBy(r => r.Hash);
        }

        /// <summary>
        /// Searches the references results for possible pull requests.
        /// </summary>
        /// <param name="references">
        /// <see cref="IEnumerable{T}"/> of <see cref="string"/> URL reference
        /// related to the advisory.
        /// </param>
        /// <returns>
        /// <see cref="IEnumerable{T}"/> of <see cref="PullRequest"/>
        /// </returns>
        private static IEnumerable<PullRequest> SearchReferencesForPullRequests(IEnumerable<string> references)
        {
            List<PullRequest> results = new();
            try
            {
                results.AddRange(UrlHelper.AnalyzeForPullUrls(references));
            }
            catch(Exception ex)
            {
                Console.WriteLine($"SearchReferencesForPullRequests Error: {ex}");
                throw;
            }

            return results.DistinctBy(r => r.Id);
        }

        /// <summary>
        /// Searches the references results for Google Git commits.
        /// </summary>
        /// <param name="references">
        /// <see cref="IEnumerable{T}"/> of <see cref="string"/> URL reference
        /// related to the advisory.
        /// </param>
        /// <returns>
        /// <see cref="IEnumerable{T}"/> of <see cref="string"/>
        /// </returns>
        private static IEnumerable<string> SearchReferencesForGoogleGitCommits(IEnumerable<string> references)
        {
            List<string> results = new();
            try
            {
                results.AddRange(UrlHelper.AnalyzeForGoogleGitCommits(references));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SearchReferencesForGoogleGitCommits Error: {ex}");
                throw;
            }

            return results.Distinct();
        }

        /// <summary>
        /// Gets an advisory from GitHub.
        /// </summary>
        /// <param name="repo">
        /// The repo to find an advisory for.
        /// </param>
        /// <param name="cve">
        /// The CVE which triggered the advisory.
        /// </param>        
        /// <returns>
        /// <see cref="Advisory"/>
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown if an unexpected error occurs.
        /// </exception>
        private async Task<Advisory?> GetAdvisoryAsync(string repo, string cve)
        {
            Advisory? advisory = null;

            try
            {
                NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
                query["cve"] = cve;
                query["repo"] = repo;

                HttpResponseMessage response = await httpClient.GetAsync($"advisories?{query}");
                
                if (response.StatusCode == HttpStatusCode.Forbidden) // Rate limit hit.
                {
                    throw new RateLimitException("Rate limit has been reached.");
                }

                response.EnsureSuccessStatusCode();

                string r = await response.Content.ReadAsStringAsync();
                AdvisoryMessages? advisories = JsonConvert.DeserializeObject<AdvisoryMessages>(await response.Content.ReadAsStringAsync());

                if (advisories != null && advisories.Advisories != null && advisories.Advisories.Any())
                {
                    advisory = advisories.Advisories.First();
                }
            }
            catch (RateLimitException ex)
            {
                Console.WriteLine($"GetAdvisoryAsync Warning: {ex}");
                throw;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"GetAdvisoryAsync HTTP Error: {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAdvisoryAsync Error: {ex}");
                throw;
            }

            return advisory;
        }

        /// <summary>
        /// Gets the commit from GitHub.
        /// </summary>
        /// <param name="owner">
        /// The owner of the repository.
        /// </param>
        /// <param name="repo">
        /// The repository the commit is for.
        /// </param>
        /// <param name="commitRef">
        /// The commit hash.
        /// </param>
        /// <returns>
        /// <see cref="GitHubCommit"/>
        /// </returns>
        private async Task<GitHubCommit?> GetCommitAsync(string owner, string repo, string commitRef)
        {
            GitHubCommit? commit = null;

            try
            {
                NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
                query["owner"] = owner;
                query["repo"] = repo;
                query["ref"] = commitRef;

                HttpResponseMessage response = await httpClient.GetAsync($"commit?{query}");

                if (response.StatusCode == HttpStatusCode.Forbidden) // Rate limit hit.
                {
                    throw new RateLimitException("Rate limit has been reached.");
                }

                response.EnsureSuccessStatusCode();

                commit = JsonConvert.DeserializeObject<GitHubCommit>(await response.Content.ReadAsStringAsync());
            }
            catch (RateLimitException ex)
            {
                Console.WriteLine($"GetCommitAsync Warning: {ex}");
                throw;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"GetCommitAsync HTTP Error: {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCommitAsync Error: {ex}");
                throw;
            }

            return commit;
        }

        /// <summary>
        /// Gets a pull request from GitHub.
        /// </summary>
        /// <param name="owner">
        /// The owner of the repo.
        /// </param>
        /// <param name="repo">
        /// The name of the repo.
        /// </param>
        /// <param name="pullNumber">
        /// The pull request id.
        /// </param>
        /// <returns>
        /// <see cref="GitHubPullRequest"/>
        /// </returns>
        private async Task<GitHubPullRequest?> GetPullRequestAsync(string owner, string repo, int pullNumber)
        {
            GitHubPullRequest? pullRequest = null;

            try
            {
                NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
                query["owner"] = owner;
                query["repo"] = repo;
                query["pullNumber"] = pullNumber.ToString();

                HttpResponseMessage response = await httpClient.GetAsync($"pull?{query}");

                if (response.StatusCode == HttpStatusCode.Forbidden) // Rate limit hit.
                {
                    throw new RateLimitException("Rate limit has been reached.");
                }

                response.EnsureSuccessStatusCode();

                pullRequest = JsonConvert.DeserializeObject<GitHubPullRequest>(await response.Content.ReadAsStringAsync());
            }
            catch (RateLimitException ex)
            {
                Console.WriteLine($"GetPullRequestAsync Warning: {ex}");
                throw;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"GetPullRequestAsync HTTP Error: {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetPullRequestAsync Error: {ex}");
                throw;
            }

            return pullRequest;
        }

        /// <summary>
        /// Gets a relase from GitHub.
        /// </summary>
        /// <param name="owner">
        /// The owner of the repository.
        /// </param>
        /// <param name="repo">
        /// The repository the commit is for.
        /// </param>
        /// <param name="tag">
        /// The tag for the releaes.
        /// </param>
        /// <returns>
        /// <see cref="GitHubRelease"/>
        /// </returns>
        private async Task<GitHubRelease?> GetRelaseAsync(string owner, string repo, string tag)
        {
            GitHubRelease? release = null;

            try
            {
                NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
                query["owner"] = owner;
                query["repo"] = repo;
                query["tag"] = tag;

                HttpResponseMessage response = await httpClient.GetAsync($"release?{query}");
                response.EnsureSuccessStatusCode();

                release = JsonConvert.DeserializeObject<GitHubRelease>(await response.Content.ReadAsStringAsync());
            }
            catch (RateLimitException ex)
            {
                Console.WriteLine($"GetRelaseAsync Warning: {ex}");
                throw;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"GetRelaseAsync HTTP Error: {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetRelaseAsync Error: {ex}");
                throw;
            }

            return release;
        }
    }
}
