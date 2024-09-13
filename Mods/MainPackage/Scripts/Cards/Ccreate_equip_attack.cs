using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Ccreate_equip_attack : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.FunctionUse = UseFunction;
        }

        private void UseFunction(BaseRole user, List<BaseRole> targets, params object[] datas)
        {
            if (StaticInstance.currWindow is BattleScene bs)
            {
                bs.DealDamage(5 + user.CurrCraftEquip, StaticEnums.AttackType.Physics, user, targets);
            }
        }
    }
}
