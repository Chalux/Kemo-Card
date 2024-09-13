using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cshadow_shot : BaseCardScript
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
                        bs.DealDamage(3 + user.CurrEffeciency * 2, StaticEnums.AttackType.Magic, user, targets, StaticEnums.AttributeEnum.DARK);
                    }
                }
            });
        }
    }
}
