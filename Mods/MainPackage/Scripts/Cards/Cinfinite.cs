using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cinfinite : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.FunctionUse = ActionFunc;
        }

        private void ActionFunc(BaseRole user, List<BaseRole> targets, dynamic[] datas)
        {
            if (targets != null && targets.Count > 0)
            {
                if (StaticUtils.TryGetBattleScene() is BattleScene bs)
                {
                    bs.DealDamage(user.Body + user.MagicAbility + user.Knowlegde + user.Technique, StaticEnums.AttackType.Physics, user, targets);
                }
            }
        }
    }
}
