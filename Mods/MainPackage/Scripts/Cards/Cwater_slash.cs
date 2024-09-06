using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cwater_slash : BaseCardScript
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
                        bs.DealDamage(7, StaticEnums.AttackType.Magic, user, targets, StaticEnums.AttributeEnum.WATER);
                    }
                }
            });
        }
    }
}
