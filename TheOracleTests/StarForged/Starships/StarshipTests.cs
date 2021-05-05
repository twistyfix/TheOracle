﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TheOracle.GameCore.Oracle;

namespace TheOracle.StarForged.Starships.Tests
{
    [TestClass()]
    public class StarshipTests
    {
        [TestMethod()]
        public void GenerateShipTest()
        {
            var services = new ServiceCollection().AddSingleton(new OracleService().Load()).BuildServiceProvider();

            var ship2 = Starship.GenerateShip(services, SpaceRegion.Outlands, $"ship-59", 0);

            for (int i = 0; i < 1000; i++)
            {
                try
                {
                    var ship = Starship.GenerateShip(services, SpaceRegion.Outlands, $"ship-{i}", 0);
                    ship.AddMission();

                    ship.GetEmbedBuilder();
                }
                catch (Exception)
                {
                    Console.WriteLine(i);
                    throw;
                }
            }
        }
    }
}