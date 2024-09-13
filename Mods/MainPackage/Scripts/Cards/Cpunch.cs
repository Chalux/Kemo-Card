using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cpunch : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.UseFilter = new((user, targets, datas) =>
            {
                return targets != null && targets.Count > 0 && targets[0] is EnemyRole er && er.isFriendly == false;
            });
            c.FunctionUse = new((user, targets, datas) =>
            {
                if (StaticInstance.currWindow is BattleScene bs)
                {
                    if (BattleStatic.isFighting)
                    {
                        bs.DealDamage(5, StaticEnums.AttackType.Physics, user, targets);
                    }
                }
            });
        }
    }
}
