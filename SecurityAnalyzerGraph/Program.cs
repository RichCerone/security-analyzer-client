using Microsoft.Extensions.Configuration;
using SecurityAnalyzer.DataModels.GitHub;
using SecurityAnalyzerGraph;
using SecurityAnalyzerGraph.Helpers;
using SecurityAnalyzerGraph.Services;

IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

// Load appsettings values.
GraphType graphType = Enum.Parse<GraphType>(configuration.GetSection("GraphType").Value);
bool isAscending = bool.Parse(configuration.GetSection("AscendingOrder").Value);
double valueLimit = int.Parse(configuration.GetSection("ValueLimit").Value);
string path = @$"{configuration.GetSection("ReportPath").Value}";

// Read in values and get sample.
IEnumerable <GitHubSecurityAnalysis> analyses = await ReportReader.ReadAnalysesAsync(path);
IEnumerable<double> values = Enumerable.Empty<double>();
switch (graphType)
{
    case GraphType.DaysResolved:

        values = CoordinateGenerator.GenerateMedianClosurePerCve(analyses, ascending: isAscending).Select(c => c.TimeToClose); 
        break;

    case GraphType.MostAffectedRepo:
        values = CoordinateGenerator.GenerateMostAffectedRepos(analyses, ascending: isAscending).Where(c => c.NumberOfCves > 1).Select(c => c.NumberOfCves);
        break;

    case GraphType.RepoWithMostCves:
        values = CoordinateGenerator.GenerateMostAffectedRepos(analyses, ascending: isAscending).Select(c => c.NumberOfCves);
        break;

    case GraphType.MostAffectiveCve:
        values = CoordinateGenerator.GenerateMostAffectiveCves(analyses, ascending: isAscending).Select(c => c.NumberOfRepos);
        break;
}

// Calculate min, max, median and standard deviation.
Console.WriteLine($"Min: {Statistics.GetMin(values)}");
Console.WriteLine($"Max: {Statistics.GetMax(values)}");
Console.WriteLine($"Median: {Statistics.GetMedian(values)}");
Console.WriteLine($"Standard Deviation: {Statistics.GetStandardDeviation(values)}");

// Build graph.
switch (graphType)
{
    case GraphType.DaysResolved:
        GraphBuilder.BuildDaysResolvedGraph(analyses, ascending: true, valueLimit);
        break;

    case GraphType.MostAffectedRepo:
        GraphBuilder.BuildMostAffectedRepoGraph(analyses, ascending: true, valueLimit);
        break;

    case GraphType.RepoWithMostCves:
        GraphBuilder.BuildRepoWithMostCvesGraph(analyses, ascending: true, valueLimit);
        break;

    case GraphType.MostAffectiveCve:
        GraphBuilder.BuildMostAffectiveCveGraph(analyses, ascending: true, valueLimit);
        break;
}