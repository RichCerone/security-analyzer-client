using SecurityAnalyzer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAnalyzer
{
    /// <summary>
    /// The configuration to use for this application.
    /// </summary>
    internal class Configuration
    {
        /// <summary>
        /// The security specs to analyze.
        /// </summary>
        public IEnumerable<SecuritySpec>? SecuritySpecs { get; set; }
    }
}
