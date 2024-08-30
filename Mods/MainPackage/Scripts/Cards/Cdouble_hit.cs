using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System;
using System.Collections.Generic;
using static BattleScene;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cdouble_hit : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.Cost = 0;
            c.Alias = "连击";
            c.Desc = "无法打出。当被丢弃时，对一名随机的敌人造成 单体/物理/无属性/14点伤害";
            c.CostType = StaticEnums.CostType.HEALTH;
            c.TargetType = StaticEnums.TargetType.ENEMY_ALL;
            c.UseFilter = new((_, _, _) => false);
            c.DiscardAction = new((BaseRole user, DisCardReason reason) =>
            {
                if (reason == DisCardReason.EFFECT)
                {
                    if (StaticInstance.currWindow is BattleScene bs)
                    {
                        if (BattleStatic.isFighting)
                        {
                            List<BaseRole> tars = new();
                            Random r = new();
                            tars.Add(bs.currentEnemyRoles[r.Next(bs.currentEnemyRoles.Count)]);
                            bs.DealDamage(14, StaticEnums.AttackType.Physics, user, tars, StaticEnums.AttributeEnum.None);
                        }
                    }
                }
                GD.Print($"卡牌C{c.Id}已使用");
            });
        }
    }
}
