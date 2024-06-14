using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class C10006 : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.Cost = 1;
            c.CardName = "无属性魔攻";
            c.Desc = "造成 敌方/单体/魔法/无属性/5点伤害";
            c.TargetType = StaticEnums.TargetType.ENEMY_SINGLE;
            c.CostType = StaticEnums.CostType.ACTIONPOINT;
            c.UseFilter = new((user, targets, datas) =>
            {
                return targets != null && targets.Count > 0 && targets[0] is InFightPlayer && targets[0].isFriendly == false;
            });
            c.FunctionUse = new((user, targets, datas) =>
            {
                if (StaticInstance.currWindow is BattleScene bs)
                {
                    if (bs.isFighting)
                    {
                        bs.DealDamage(5, StaticEnums.AttackType.Physics, user, targets);
                    }
                }
                GD.Print($"卡牌C{c.Id}已使用");
            });
        }
    }
}
