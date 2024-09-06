using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Crock_drill : BaseCardScript
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
                        if (targets[0] is EnemyRole er)
                        {
                            if (er.CurrPBlock > 0)
                            {
                                bs.DealDamage(8, StaticEnums.AttackType.Physics, user, targets, StaticEnums.AttributeEnum.EARTH);
                            }
                        }
                        bs.DealDamage(4, StaticEnums.AttackType.Magic, user, targets, StaticEnums.AttributeEnum.EARTH);
                    }
                }
            });
        }
    }
}
