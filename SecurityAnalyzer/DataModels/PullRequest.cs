
namespace SecurityAnalyzer.DataModels
{
    /// <summary>
    /// Contains generic pull request information.
    /// </summary>
    internal class PullRequest
    {
        /// <summary>
        /// The id of the pull request.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The owner of the repo.
        /// </summary>
        public string RepoOwner { get; set; } = string.Empty;

        /// <summary>
        /// The repo the pull request is for.
        /// </summary>
        public string Repo { get; set; } = string.Empty;

        /// <summary>
        /// Creates a new <see cref="PullRequest"/>.
        /// </summary>
        /// <param name="id">
        /// The id of the pull request.
        /// </param>
        /// <param name="repoOwner">
        /// The owner of the repo.
        /// </param>
        /// <param name="repo">
        /// The repo the pull request is for.
        /// </param>
        public PullRequest(int id, string repoOwner, string repo)
        {
            Id = id;
            RepoOwner = repoOwner;
            Repo = repo;
        }
    }
}
