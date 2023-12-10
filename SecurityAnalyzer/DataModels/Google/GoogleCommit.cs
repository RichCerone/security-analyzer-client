namespace SecurityAnalyzer.DataModels.Google
{
    /// <summary>
    /// Represents a Google commit.
    /// </summary>
    public class GoogleCommit
    {
        /// <summary>
        /// The commiter who made the commit.
        /// </summary>
        public string Comitter { get; set; } = string.Empty;

        /// <summary>
        /// The date and time the commit was made.
        /// </summary>
        public DateTime? DateTime { get; set; }
    }
}
