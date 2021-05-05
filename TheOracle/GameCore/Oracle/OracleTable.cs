using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.IronSworn;

namespace TheOracle.GameCore.Oracle
{
    public class OracleTable
    {
        public string[] Aliases { get; set; }
        public int d { get; set; } = 100;
        public string Category { get; set; }
        public string DisplayMode { get; set; }
        public bool DisplayChances { get; set; } = true;
        public GameName? Game { get; set; }
        public string Name { get; set; }
        public List<StandardOracle> Oracles { get; set; }
        public string Pair { get; set; } = string.Empty;
        public bool ShowResult { get; set; } = true;
        public string Type { get; set; }
        public OracleRequirement Requires { get; set; }

        public bool MatchTableAlias(string table)
        {
            if (Requires != null)
            {
                if (!Requires.Value.Any(req => table.Contains(req))) return false;
                foreach (string s in Requires.Value) table = table.Replace(s, "");
                table = System.Text.RegularExpressions.Regex.Replace(table, "  +", " ").Trim();
            }

            string cleanTableName = table;
            if (Category != null && table.Contains(Category))
            {
                cleanTableName = cleanTableName.Replace(Category, "").Replace("  ", " ").Trim();
            }

            return this.Name.Equals(table, StringComparison.OrdinalIgnoreCase) || this.Name.Equals(cleanTableName, StringComparison.OrdinalIgnoreCase) || this.Aliases?.Any(alias => alias.Equals(table, StringComparison.OrdinalIgnoreCase)) == true;
        }

        internal bool ContainsTableAlias(string tableName, string[] additionalSearchTerms)
        {
            string ogName = tableName;
            if (Requires != null)
            {
                if (!Requires.Value.Any(req => tableName.Contains(req) || additionalSearchTerms?.Contains(req) == true)) return false;
                foreach (string s in Requires.Value) tableName = tableName.Replace(s, "");
                tableName = System.Text.RegularExpressions.Regex.Replace(tableName, "  +", " ").Trim();
            }

            if (Category != null && tableName.Contains(Category))
            {
                tableName = tableName.Replace(Category, "").Replace("  ", " ").Trim();
            }

            if (tableName.Length < 3) return false;

            var temp1 = this.Name.Contains(tableName, StringComparison.OrdinalIgnoreCase) || this.Aliases?.Any(alias => alias.Contains(tableName, StringComparison.OrdinalIgnoreCase)) == true;
            var temp2 = additionalSearchTerms.Any(s => Name.Contains(s) || Aliases?.Contains(s) == true || Requires?.Value.Any(req => req.Contains(s)) == true);

            if (temp1 && temp2)
            {
                System.Threading.Thread.Sleep(1);
            }

            return temp1 && temp2;
        }
    }

    public class OracleRequirement
    {
        public string Property { get; set; }
        public string[] Value { get; set; }
    }

}