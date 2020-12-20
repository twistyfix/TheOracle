using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TheOracle.GameCore.Assets;
using TheOracle.GameCore.Oracle;

namespace TheOracle.GameCore.UserContent
{
    public class UserSubscriptions
    {
        [Key]
        public ulong UserId { get; set; }
        public List<Asset> Assets { get; set; }
        public List<StandardOracle> Oracles { get; set; }
    }
}
