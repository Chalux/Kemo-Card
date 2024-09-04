using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cmag_draw : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.Cost = 6;
            c.Alias = "魔能";
            c.Desc = "造成 敌方/单体/魔法/无属性/5点伤害，如果此卡对目标造成了伤害，则抽一张卡";
            c.TargetType = StaticEnums.TargetType.ENEMY_SINGLE;
            c.CostType = StaticEnums.CostType.MAGIC;
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
                        bs.DealDamage(5, StaticEnums.AttackType.Magic, user, targets, StaticEnums.AttributeEnum.None);
                        if ((targets[0] as EnemyRole).CurrHealth < oldHealth && user is PlayerRole pr)
                        {
                            bs.DrawCard(1, pr);
                        }
                    }
                }
                GD.Print($"卡牌C{c.Id}已使用");
            });
        }
    }
}
