
namespace SecurityAnalyzerGraph
{
    /// <summary>
    /// Settings for configuring the application.
    /// </summary>
    internal class AppSettings
    {
        /// <summary>
        /// The graph type for the application to build.
        /// </summary>
        /// <remarks>
        /// Builds DaysResolved by default.
        /// </remarks>
        public GraphType GraphType { get; set; } = GraphType.DaysResolved;

        /// <summary>
        /// Whether to use asencding order when organizing the graph.
        /// </summary>
        /// <see cref="true"/> by default.
        public bool AscendingOrder { get; set; } = true;

        /// <summary>
        /// The value limit to display on the graph.
        /// </summary>
        /// <remarks>
        /// Set to 0 by default.
        /// </remarks>
        /// <example>
        /// If value is 25, any values greater than 25, will be only displayed as 25.
        /// </example>
        public double ValueLimit { get; set; } = 0;

        /// <summary>
        /// The absolute path to the report JSON.
        /// </summary>
        public string ReportPath { get; set; } = string.Empty;
    }
}
