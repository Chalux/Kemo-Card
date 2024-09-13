using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Ccreate_equip_armor : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.FunctionUse = UseFunction;
        }

        private void UseFunction(BaseRole user, List<BaseRole> targets, params object[] datas)
        {
            if (StaticInstance.currWindow is BattleScene bs)
            {
                (user as PlayerRole).CurrPBlock += 2 + user.CurrCraftEquip;
            }
        }
    }
}
