using ScottPlot;
using ScottPlot.Plottable;
using SecurityAnalyzer.DataModels.GitHub;
using SecurityAnalyzerGraph.DataModels;
using SecurityAnalyzerGraph.Services;

IEnumerable<GitHubSecurityAnalysis> analyses = await ReportReader.ReadAnalysesAsync();

List<ClosureResponse> coordinates = CoordinateGenerator.GenerateClosureResponseForCve(analyses);

IEnumerable<string> yLabels = coordinates.Select(y => y.RepoName);
IEnumerable<double> yAxis = coordinates.Select(y => y.TimeToClose);

IEnumerable<string> xLabels = coordinates.Select(x => x.Cve);
IEnumerable<double> xAxis = coordinates.Select(x => (double)coordinates.IndexOf(x));

xAxis = DataGen.Consecutive(xAxis.Count());

Plot plot = new(600, 400);

var scatter = plot.AddScatter(xAxis.ToArray(), yAxis.ToArray());

plot.XAxis.ManualTickPositions(xAxis.ToArray(), xLabels.ToArray());
plot.YAxis.ManualTickPositions(yAxis.ToArray(), yAxis.Select(y => y.ToString()).ToArray());


plot.XAxis.SetSizeLimit(min: 50);
plot.XAxis.TickLabelStyle(rotation: 45);

scatter.DataPointLabels = yLabels.ToArray();

new FormsPlotViewer(plot).ShowDialog();