using System;
using System.Collections.Generic;
using System.Text;

namespace MSI
{
    public class Configuration
    {
        public int MaxIterations { get; set; }
        public string PathToProblem { get; set; }
        public int PopulationSize { get; set; }
        public string OutputPath { get; set; }
    }
}
