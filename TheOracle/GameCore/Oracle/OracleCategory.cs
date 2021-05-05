using System.Collections.Generic;

namespace TheOracle.GameCore.Oracle
{
    internal class OracleCategory
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string[] Aliases { get; set; }
        public SourceInfo Source { get; set; }
        public List<OracleTable> Oracles { get; set; }
    }
}