using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using TheOracle.Core;
using TheOracle.GameCore.Oracle;
using TheOracle.IronSworn;

namespace TheOracle.GameCore.Oracle
{
    public interface IOracleEntry
    {
        int Chance { get; set; }
        string Description { get; set; }
        string Prompt { get; set; }
    }
    public enum OracleType 
    {
        standard,
        nested,
        multipleColumns,
        paired
    }
}