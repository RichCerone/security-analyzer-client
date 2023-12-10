using ScottPlot;
using ScottPlot.Plottable;
using SecurityAnalyzer.DataModels.GitHub;
using SecurityAnalyzerGraph.DataModels;
using System.Text;

namespace SecurityAnalyzerGraph.Services
{
    internal static class GraphBuilder
    {
        /// <summary>
        /// Builds a graph showing the average closure time to
        /// resolve a CVE for a repo.
        /// </summary>
        /// <param name="analyses">
        /// <see cref="IEnumerable{T}"/> of <see cref="GitHubSecurityAnalysis"/> 
        /// to graph.
        /// </param>
        /// <param name="ascending">
        /// Whether the graph should be ordered by ascending.
        /// </param>
        /// <param name="valueLimit">
        /// Value limit to place on the x-axis.
        /// </param>
        public static void BuildDaysResolvedGraph(IEnumerable<GitHubSecurityAnalysis> analyses, bool ascending, double valueLimit = 0)
        {
            List<AverageClosure> coordinates = CoordinateGenerator.GenerateMedianClosurePerCve(analyses, ascending);            

            Plot plot = new(600, 400);
            plot.Style(Style.Seaborn);

            List<Bar> bars = new();
            List<double> yAxis = new();
            List<string> xLabels = new();
            for (int i = 0; i < coordinates.Count; i++)
            {
                string cve = coordinates.ElementAt(i).Cve;
                AverageClosure averageClosure = coordinates[i];

                StringBuilder label = new();
                foreach (string repo in averageClosure.AffectedRepos)
                {
                    label.Append($"{repo},");
                }

                Bar bar = new()
                {
                    Value = averageClosure.TimeToClose >= valueLimit ? valueLimit : averageClosure.TimeToClose,
                    Position = i,
                    FillColor = Palette.Category10.GetColor(i),
                    //Label = $"[{label.ToString().TrimEnd(',')}] : {averageClosure.TimeToClose}",
                    IsVertical = false,
                    LineWidth = 2,
                };

                bars.Add(bar);
                yAxis.Add(i);
                xLabels.Add(string.Empty);
            }

            plot.AddBarSeries(bars);
            plot.YAxis.ManualTickPositions(yAxis.ToArray(), xLabels.ToArray());
            plot.SetAxisLimits(xMax: valueLimit);
            plot.YAxis.SetSizeLimit(min: 50);
            //plot.YAxis.Label("CVE"); 
            
            plot.XAxis.Label("Time to Close (In Days)");
            plot.XTicks(positions: new double[] { 0, 10, 100, 1000, 10000, 100000 }, labels: new string[] { "0", "10", "100", "1000", "10000", "100000"});

            new FormsPlotViewer(plot).ShowDialog();
        }

        /// <summary>
        /// Builds a graph showing how many repos were 
        /// affected by CVE's.
        /// </summary>
        /// <param name="analyses">
        /// <see cref="IEnumerable{T}"/> of <see cref="GitHubSecurityAnalysis"/> 
        /// to graph.
        /// </param>
        /// <param name="ascending">
        /// Whether the graph should be ordered by ascending.
        /// </param>
        /// <param name="valueLimit">
        /// Value limit to place on the x-axis.
        /// </param>
        public static void BuildMostAffectedRepoGraph(IEnumerable<GitHubSecurityAnalysis> analyses, bool ascending, double valueLimit = 0)
        {
            List<MostAffectedRepo> coordinates = CoordinateGenerator.GenerateMostAffectedRepos(analyses, ascending);

            Plot plot = new(600, 400);
            plot.Style(Style.Seaborn);

            List<Bar> bars = new();
            List<double> yAxis = new();
            List<string> xLabels = new();
            for (int i = 0; i < coordinates.Count; i++)
            {
                MostAffectedRepo mostAffected = coordinates[i];

                if (mostAffected.NumberOfCves > 1)
                {
                    Bar bar = new()
                    {
                        Value = mostAffected.NumberOfCves >= valueLimit ? valueLimit : mostAffected.NumberOfCves,
                        Position = i,
                        FillColor = Palette.Category10.GetColor(i),
                        Label = mostAffected.NumberOfCves.ToString(),
                        IsVertical = false,
                        LineWidth = 2,
                    };

                    bars.Add(bar);
                    yAxis.Add(i);
                    xLabels.Add(mostAffected.RepoName);
                }                
            }

            plot.AddBarSeries(bars);
            plot.YAxis.ManualTickPositions(yAxis.ToArray(), xLabels.ToArray());
            plot.SetAxisLimits(xMax: valueLimit);
            plot.YAxis.SetSizeLimit(min: 50);
            plot.YAxis.Label("Repo Name");

            plot.XAxis.Label("Number of CVE's");
            plot.XAxis.ManualTickSpacing(1);
            
            new FormsPlotViewer(plot).ShowDialog();
        }

        /// <summary>
        /// Builds a graph showing how many repos were 
        /// affected by CVE's. Differs from <see cref="BuildMostAffectedRepoGraph"/>
        /// because it does not exclude repos with 1 or less CVE's.
        /// </summary>
        /// <param name="analyses">
        /// <see cref="IEnumerable{T}"/> of <see cref="GitHubSecurityAnalysis"/> 
        /// to graph.
        /// </param>
        /// <param name="ascending">
        /// Whether the graph should be ordered by ascending.
        /// </param>
        /// <param name="valueLimit">
        /// Value limit to place on the x-axis.
        /// </param>
        public static void BuildRepoWithMostCvesGraph(IEnumerable<GitHubSecurityAnalysis> analyses, bool ascending, double valueLimit = 0)
        {
            List<MostAffectedRepo> coordinates = CoordinateGenerator.GenerateMostAffectedRepos(analyses, ascending);

            Plot plot = new(600, 400);
            plot.Style(Style.Seaborn);

            List<Bar> bars = new();
            List<double> yAxis = new();
            List<string> xLabels = new();
            for (int i = 0; i < coordinates.Count; i++)
            {
                MostAffectedRepo mostAffected = coordinates[i];

                Bar bar = new()
                {
                    Value = mostAffected.NumberOfCves >= valueLimit ? valueLimit : mostAffected.NumberOfCves,
                    Position = i,
                    FillColor = Palette.Category10.GetColor(i),
                    Label = mostAffected.NumberOfCves.ToString(),
                    IsVertical = false,
                    LineWidth = 2,
                };

                bars.Add(bar);
                yAxis.Add(i);
                xLabels.Add(mostAffected.RepoName);
            }

            plot.AddBarSeries(bars);
            plot.YAxis.ManualTickPositions(yAxis.ToArray(), xLabels.ToArray());
            plot.SetAxisLimits(xMax: valueLimit);
            plot.YAxis.SetSizeLimit(min: 50);
            plot.YAxis.Label("Repo Name");

            plot.XAxis.Label("Number of CVE's");
            plot.XAxis.ManualTickSpacing(1);

            new FormsPlotViewer(plot).ShowDialog();
        }

        /// <summary>
        /// Builds a graph showing how many CVE's affected 
        /// the number of repos.
        /// </summary>
        /// <param name="analyses">
        /// <see cref="IEnumerable{T}"/> of <see cref="GitHubSecurityAnalysis"/> 
        /// to graph.
        /// </param>
        /// <param name="ascending">
        /// Whether the graph should be ordered by ascending.
        /// </param>
        /// <param name="valueLimit">
        /// Value limit to place on the x-axis.
        /// </param>
        public static void BuildMostAffectiveCveGraph(IEnumerable<GitHubSecurityAnalysis> analyses,bool ascending, double valueLimit = 0)
        {
            List<MostAffectiveCve> coordinates = CoordinateGenerator.GenerateMostAffectiveCves(analyses, ascending);

            Plot plot = new(600, 400);
            plot.Style(Style.Seaborn);

            List<Bar> bars = new();
            List<double> yAxis = new();
            List<string> xLabels = new();
            for (int i = 0; i < coordinates.Count; i++)
            {
                MostAffectiveCve mostAffected = coordinates[i];

                Bar bar = new()
                {
                    Value = mostAffected.NumberOfRepos >= valueLimit ? valueLimit : mostAffected.NumberOfRepos,
                    Position = i,
                    FillColor = Palette.Category10.GetColor(i),
                    Label = mostAffected.NumberOfRepos.ToString(),
                    IsVertical = false,
                    LineWidth = 2,
                };

                bars.Add(bar);
                yAxis.Add(i);
                xLabels.Add(mostAffected.Cve);
            }

            plot.AddBarSeries(bars);
            plot.YAxis.ManualTickPositions(yAxis.ToArray(), xLabels.ToArray());
            plot.SetAxisLimits(xMax: valueLimit);
            plot.YAxis.SetSizeLimit(min: 50);
            plot.YAxis.Label("CVE");

            plot.XAxis.Label("Number of Repos");

            new FormsPlotViewer(plot).ShowDialog();
        }
    }
}
