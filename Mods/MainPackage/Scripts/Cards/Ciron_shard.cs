using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using System;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Ciron_shard : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.FunctionUse = UseFunction;
        }

        private void UseFunction(BaseRole user, List<BaseRole> targets, dynamic[] datas)
        {
            Card card = datas[0] as Card;
            (user as PlayerRole).CurrPBlock += (int)Math.Max(0, 10 - card.InGameDict.GetValueOrDefault("usecount", 0));
            card.InGameDict["usecount"] = card.InGameDict.GetValueOrDefault("usecount", 0) + 1;
        }
    }
}
