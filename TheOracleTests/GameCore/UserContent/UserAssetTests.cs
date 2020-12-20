using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TheOracle.GameCore;
using TheOracle.GameCore.Assets;

namespace TheOracleTests.GameCore.UserContent
{
    [TestClass]
    public class UserAssetTests
    {
        [TestMethod]
        public void CreateCustomAsset()
        {
            using var db = new ThirdPartyContentContext();

            Asset asset = new Asset();
            asset.Name = "Test";
            asset.Game = GameName.Ironsworn;
            asset.Description = "Test asset for 3rd party content";
            asset.AssetFields = new List<AssetField>
            {
                new AssetField{Enabled = true, Text = "Field 1"},
                new AssetField{Enabled = false, Text = "Field 2"},
                new AssetField{Enabled = false, Text = "Field 3"},
            };

            db.UserAssets.Add(asset);
            db.UserSubscriptions.Add(new TheOracle.GameCore.UserContent.UserSubscriptions { UserId = 1, Assets = new List<Asset> { asset } });
            db.SaveChangesAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public void GetCustomAsset()
        {
            var assetService = new AssetService(new List<Asset>());

            var userAssets = assetService.GetUserAssetsAsync(1).Result;

            Assert.IsTrue(userAssets.Count > 0);

        }
    }
}