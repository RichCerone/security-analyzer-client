using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAnalyzerGraph.DataModels
{
    internal class ClosureResponse
    {
        public string RepoName { get; set; } = string.Empty;

        public string Cve { get; set; } = string.Empty;

        public double TimeToClose { get; set; }
    }
}
