
namespace SecurityAnalyzer.DataModels
{
    /// <summary>
    /// Contains generic commit hash information.
    /// </summary>
    internal class CommitHash
    {
        /// <summary>
        /// The commit hash or reference.
        /// </summary>
        public string Hash { get; set; } = string.Empty;

        /// <summary>
        /// The owner of the repo.
        /// </summary>
        public string RepoOwner { get; set; } = string.Empty;

        /// <summary>
        /// The repo the commit is for.
        /// </summary>
        public string Repo {  get; set; } = string.Empty;

        /// <summary>
        /// Creates a new <see cref="CommitHash"/>.
        /// </summary>
        /// <param name="hash">
        /// The commit hash or reference.
        /// </param>
        /// <param name="repoOwner">
        /// The owner of the repo.
        /// </param>
        /// <param name="repo">
        /// The repo the commit is for.
        /// </param>
        public CommitHash(string hash, string repoOwner, string repo) 
        { 
            Hash = hash;
            RepoOwner = repoOwner;
            Repo = repo;
        }
    }
}
