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
    internal partial class Cno_defense : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.Cost = 1;
            c.Alias = "弃守为攻";
            c.Desc = "丢弃手牌中所有护甲/魔防牌，每丢弃一张对敌方单体造成1次无属性物理4点伤害";
            c.TargetType = StaticEnums.TargetType.ENEMY_SINGLE;
            c.CostType = StaticEnums.CostType.ACTIONPOINT;
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
                        var pr = user as PlayerRole;
                        List<Card> cards = new();
                        foreach (var card in pr.InFightHands)
                        {
                            if ((card.FilterFlags & (ulong)StaticEnums.CardFlag.ARMOR) != 0)
                            {
                                cards.Add(card);
                            }
                        }
                        bs.DisCard(cards, pr, BattleScene.DisCardReason.EFFECT);
                        bs.DealDamage(4, StaticEnums.AttackType.Physics, user, targets, StaticEnums.AttributeEnum.None, cards.Count);
                    }
                }
                GD.Print($"卡牌C{c.Id}已使用");
            });
        }
    }
}
