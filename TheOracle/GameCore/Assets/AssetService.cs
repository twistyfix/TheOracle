using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle.GameCore.Assets
{
    public class AssetService
    {
        private List<Asset> BaseList;

        public AssetService(List<Asset> baseList)
        {
            this.BaseList = baseList;
        }

        public async Task<List<Asset>> GetUserAssetsAsync(ulong userId)
        {
            var userAssetList = BaseList;

            var db = new ThirdPartyContentContext();
            var subs = await db.UserSubscriptions.FirstOrDefaultAsync(ua => ua.UserId == userId);

            if (subs.Assets?.Count > 0)
            {
                userAssetList.AddRange(subs.Assets);
            }

            return userAssetList;
        }
    }
}
