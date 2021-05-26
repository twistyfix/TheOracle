﻿using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.Core;

namespace TheOracle.GameCore.Oracle
{
    public class OracleService
    {
        public List<OracleTable> OracleList { get; set; }

        public OracleService()
        {
            OracleList = new List<OracleTable>();
        }

        public OracleService Load()
        {
            var ironOraclesDir = new DirectoryInfo(Path.Combine("IronSworn", "Oracles"));
            if (ironOraclesDir.Exists)
            {
                foreach (var file in ironOraclesDir.GetFiles("*.json"))
                {
                    var oracles = JsonConvert.DeserializeObject<List<OracleTable>>(File.ReadAllText(file.FullName));
                    oracles.ForEach(o => o.Game = GameName.Ironsworn);
                    OracleList.AddRange(oracles);
                }
            }

            DirectoryInfo starOraclesDir = new DirectoryInfo(Path.Combine("StarForged", "Data", "oracles"));
            if (starOraclesDir.Exists)
            {
                foreach (var file in starOraclesDir.GetFiles("*.json"))
                {
                    var oracleInfo = JsonConvert.DeserializeObject<DataSworn.OracleInfo>(File.ReadAllText(file.FullName));

                    var loader = new DataSwornTableLoader();
                    
                    foreach(var oracle in oracleInfo.Oracles)
                    {
                        OracleList.AddRange(loader.GetTables(oracleInfo, oracle, GameName.Starforged));
                    }
                }
            }

            var ironOraclesPath = Path.Combine("IronSworn", "oracles.json");
            //var starOraclesPath = Path.Combine("StarForged", "StarforgedOracles.json");
            var tarotOraclesPath = Path.Combine("IronSworn", "tarot_oracles.json");
            if (File.Exists(ironOraclesPath))
            {
                var ironSworn = JsonConvert.DeserializeObject<List<OracleTable>>(File.ReadAllText(ironOraclesPath));
                ironSworn.ForEach(o => o.Game = GameName.Ironsworn);
                OracleList.AddRange(ironSworn);
            }
            //if (File.Exists(starOraclesPath))
            //{
            //    var starForged = JsonConvert.DeserializeObject<List<OracleTable>>(File.ReadAllText(starOraclesPath));
            //    starForged.ForEach(o => o.Game = GameName.Starforged);
            //    OracleList.AddRange(starForged);
            //}
            if (File.Exists(tarotOraclesPath))
            {
                var tarot = JsonConvert.DeserializeObject<List<OracleTable>>(File.ReadAllText(tarotOraclesPath));
                OracleList.AddRange(tarot);
            }

            foreach (var oracleSet in this.OracleList)
            {
                try
                {
                    if (oracleSet.Oracles?.All(o => o.Chance == 0) ?? false)
                    {
                        for (int i = 0; i < oracleSet.Oracles.Count; i++)
                        {
                            oracleSet.Oracles[i].Chance = i + 1;
                        }
                        oracleSet.d = oracleSet.Oracles.Count;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"Error Loading oracle: {oracleSet.Name}");
                    throw;
                }
            }

            return this;
        }

        public IOracleEntry RandomRow(string TableName, GameName game = GameName.None, Random rand = null)
        {
            if (rand == null) rand = BotRandom.Instance;
            try
            {
                var item = OracleList.SingleOrDefault(ot => ot.MatchTableAlias(TableName) && (ot.Game == game || game == GameName.None))?.Oracles?.GetRandomRow(rand);

                if (item == default)
                {
                    var catMatch = OracleList.Where(ot => ot.Category != null && TableName.Contains(ot.Category, StringComparison.OrdinalIgnoreCase));
                    var match = catMatch.SingleOrDefault(ot => ot.MatchTableAlias(TableName.Replace(ot.Category, "").Trim()));
                    item = match?.Oracles?.GetRandomRow(rand);

                    if (item == default)
                    {
                        Console.WriteLine("debug");
                    }
                }

                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ArgumentException argEx = new ArgumentException($"Error retrieving oracle '{TableName}' for game '{game}'", ex);
                throw argEx;
            }
        }

        public string RandomOracleResult(string TableName, IServiceProvider serviceProvider, GameName game = GameName.None, Random rand = null)
        {
            if (rand == null) rand = BotRandom.Instance;
            var row = RandomRow(TableName, game, rand);

            var tableData = OracleList.SingleOrDefault(ot => ot.Name == TableName && (ot.Game == game || game == GameName.None));
            if (tableData == default)
            {
                var catMatch = OracleList.Where(ot => ot.Category != null && TableName.Contains(ot.Category, StringComparison.OrdinalIgnoreCase));
                tableData = catMatch.Single(ot => ot.ContainsTableAlias(TableName));
            }

            game = tableData.Game ?? GameName.None;

            string lookup = row.Description;

            var match = Regex.Match(lookup, @"\[.*(\d+)x");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int rolls))
            {
                List<string> ReplaceMultiRollTables = new List<string>();
                for (int i = 0; i < rolls; i++)
                {
                    ReplaceMultiRollTables.Add(tableData.Name);
                }
                lookup = lookup.Replace($"{match.Groups[1]}x", string.Join("/", ReplaceMultiRollTables));
            }

            var oracleService = serviceProvider.GetRequiredService<OracleService>();
            var roller = new OracleRoller(oracleService, game, rand);
            var tables = roller.ParseOracleTables(lookup);
            if (tables.Count == 0) return row.Description;
            roller.BuildRollResults(lookup);

            var finalResults = roller.RollResultList.Select(ocl => ocl.Result.Description);

            var spacer = (match.Success) ? " " : "\n";
            return $"{row.Description}{spacer}" + String.Join(" / ", finalResults);
        }

        public List<RollResult> RandomOracleResultList(string TableName, IServiceProvider serviceProvider, GameName game = GameName.None, Random rand = null, string[] additionalSearchTerms = null)
        {
            if (rand == null) rand = BotRandom.Instance;
            var row = RandomRow(TableName, game, rand);

            var tableData = OracleList.Single(ot => ot.Name == TableName && (ot.Game == game || game == GameName.None));
            game = tableData.Game ?? GameName.None;

            string lookup = row.Description;

            var match = Regex.Match(lookup, @"\[.*(\d+)x");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int rolls))
            {
                List<string> ReplaceMultiRollTables = new List<string>();
                for (int i = 0; i < rolls; i++)
                {
                    ReplaceMultiRollTables.Add(tableData.Name);
                }
                lookup = lookup.Replace($"{match.Groups[1]}x", string.Join("/", ReplaceMultiRollTables));
            }

            var oracleService = serviceProvider.GetRequiredService<OracleService>();
            var roller = new OracleRoller(oracleService, game, rand);
            var tables = roller.ParseOracleTables(lookup);

            if (tables.Count == 0)
            {
                var rollResult = new RollResult();
                rollResult.ParentTable = serviceProvider.GetRequiredService<OracleService>().OracleList.First(tbl => tbl.Name == TableName && (tbl.Game == game || game == GameName.None));
            }

            roller.BuildRollResults(lookup, additionalSearchTerms);

            var finalResults = roller.RollResultList.Select(ocl => ocl.Result.Description);

            var spacer = (match.Success) ? " " : "\n";
            return roller.RollResultList;
        }
    }
}