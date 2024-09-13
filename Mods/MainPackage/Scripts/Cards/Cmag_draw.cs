using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cmag_draw : BaseCardScript
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
                        int oldHealth = (targets[0] as EnemyRole).CurrHealth;
                        bs.DealDamage(3 + user.CurrEffeciency, StaticEnums.AttackType.Magic, user, targets, StaticEnums.AttributeEnum.None);
                        if ((targets[0] as EnemyRole).CurrHealth < oldHealth && user is PlayerRole pr)
                        {
                            bs.DrawCard(1, pr);
                        }
                    }
                }
            });
        }
    }
}
