using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TheOracle.GameCore.Oracle.Tests
{
    [TestClass()]
    public class OracleRollerTests
    {
        [TestMethod()]
        [TestCategory("Integration")]
        public void BuildRollResultsTest()
        {
            var oracleService = new OracleService().Load();

            oracleService.RandomRow("Action", GameName.Ironsworn);
            oracleService.RandomRow("Theme", GameName.Ironsworn);

            Assert.ThrowsException<ArgumentException>(() => oracleService.RandomRow("Action"));
        }

        [TestMethod()]
        public void BuildRollResultsTest1()
        {
            var services = new ServiceCollection().AddSingleton(new OracleService().Load()).BuildServiceProvider();

            for (int i = 28; i < 100; i++)
            {
                try
                {
                    var rand = new Random(i);
                    var roller = new OracleRoller(services, GameName.Starforged, rand).BuildRollResults("Space Sighting Outlands");
                    Console.WriteLine($"{i} - {string.Join(", ", roller.RollResultList.Select(rr => rr.Result.Description))}");
                }
                catch (Exception)
                {
                    Console.WriteLine($"Error on {i}");
                    throw;
                }
            }
        }

        [TestMethod()]
        public void BuildRollResultsTest2()
        {
            var services = new ServiceCollection().AddSingleton(new OracleService().Load()).BuildServiceProvider();

            var roller = new OracleRoller(services, GameName.Ironsworn);

            for (int i = 0; i < 100; i++)
            {
                roller.BuildRollResults("Site Name Format");
                Console.WriteLine(string.Join(", ", roller.RollResultList.Select(rr => rr.Result.Description)));
            }
        }

        [TestMethod()]
        public void ParseOracleTablesTest()
        {
            string[] additionalSearchTerms = new string[] { "Outlands" };
            var roller = new OracleRoller(new ServiceCollection().AddSingleton(new OracleService().Load()).BuildServiceProvider(), GameName.Starforged);

            roller.ParseOracleTables("[Starship Mission]", additionalSearchTerms);
        }
    }
}