using System;
using System.Collections.Generic;
using System.Text;
using TheOracle.GameCore.Assets;
using TheOracle.GameCore.Oracle;

namespace TheOracle.GameCore.UserContent
{
    public class UserContent
    {
        public List<Asset> Assets { get; set; }
        public List<StandardOracle> Oracles { get; set; }
    }
}
