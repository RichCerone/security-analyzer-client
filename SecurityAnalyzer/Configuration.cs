using SecurityAnalyzer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAnalyzer
{
    internal class Configuration
    {
        public IEnumerable<SecuritySpec>? SecuritySpecs { get; set; }
    }
}
