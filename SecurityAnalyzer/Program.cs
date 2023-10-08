using Microsoft.Extensions.Configuration;
using SecurityAnalyzer.DataModels.GitHub;
using SecurityAnalyzer.Services;

static async Task RunAsync()
{
    IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

    GitHubProcessor gitHubProcessor = new(configuration);

    IEnumerable<GitHubSecurityAnalysis> analyses = await gitHubProcessor.GetSecurityAnalysisAsync();
    foreach (GitHubSecurityAnalysis analysis in analyses)
    {
        if (analysis.Advisory != null)
        {
            if (analysis.Advisory.NvdePublishedAt.HasValue)
            {
                analysis.StartOfExposure = analysis.Advisory.NvdePublishedAt.Value;
            }
            else
            {
                analysis.StartOfExposure = analysis.Advisory.PublishedAt;
            }
        }
        
        if (analysis.Commit != null && analysis.Commit.Message != null)
        {
            analysis.EndOfExposure = analysis.Commit?.Message?.Commit?.Author?.Date;
        }
        else if (analysis.PullRequest != null && analysis.PullRequest.Message != null)
        {
            analysis.EndOfExposure = analysis.PullRequest?.Message?.MergedAt;
        }
        else if (analysis.Release != null && analysis.Release.Message != null)
        {
            analysis.EndOfExposure = analysis.Release?.Message?.CreatedAt;
        }
    }
    
    await ReportService.GenerateReportAsync(analyses);
}

await RunAsync();