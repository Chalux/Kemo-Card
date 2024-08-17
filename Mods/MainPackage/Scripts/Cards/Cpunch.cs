using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cpunch : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.Cost = 1;
            c.Alias = "无属性物攻";
            c.Desc = "造成 敌方/单体/物理/无属性/5点伤害";
            c.TargetType = StaticClass.StaticEnums.TargetType.ENEMY_SINGLE;
            c.CostType = StaticClass.StaticEnums.CostType.ACTIONPOINT;
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
                GD.Print($"卡牌C{c.Id}已使用");
            });
        }
    }
}
