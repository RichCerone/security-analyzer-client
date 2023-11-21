using Microsoft.Extensions.Configuration;
using SecurityAnalyzer.DataModels.GitHub;
using SecurityAnalyzer.Services;

static async Task RunAsync()
{
    IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

    GitHubProcessor gitHubProcessor = new(configuration, new GoogleGitProcessor());

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

        List<DateTime?> dates = new();
        if (analysis.GitHubCommit != null && analysis.GitHubCommit.Message != null)
        {
            dates.Add(analysis.GitHubCommit?.Message?.Commit?.Author?.Date);
        }
        else if (analysis.GoogleGitCommit != null && analysis.GoogleGitCommit.DateTime != null)
        {
            dates.Add(analysis.GoogleGitCommit.DateTime);
        }
        else if (analysis.PullRequest != null && analysis.PullRequest.Message != null)
        {
            dates.Add(analysis.PullRequest?.Message?.MergedAt);
        }
        else if (analysis.Release != null && analysis.Release.Message != null)
        {
            dates.Add(analysis.Release?.Message?.CreatedAt);
        }

        analysis.EndOfExposure = dates.OrderByDescending(d => d).FirstOrDefault();
    }
    
    await ReportService.GenerateReportAsync(analyses);
}

await RunAsync();