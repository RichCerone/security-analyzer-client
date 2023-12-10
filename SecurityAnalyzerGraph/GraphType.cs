using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAnalyzerGraph
{
    /// <summary>
    /// The type of graph to analze and build.
    /// </summary>
    public enum GraphType
    {
        DaysResolved,
        MostAffectedRepo,
        RepoWithMostCves,
        MostAffectiveCve
    }
}
