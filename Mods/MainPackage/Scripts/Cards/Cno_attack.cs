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
    internal partial class Cno_attack : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.Cost = 1;
            c.Alias = "弃攻为守";
            c.Desc = "丢弃手牌中所有攻击牌，每丢弃一张获得3点护甲和3点魔防";
            c.TargetType = StaticEnums.TargetType.SELF;
            c.CostType = StaticEnums.CostType.ACTIONPOINT;
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
                            if ((card.FilterFlags & (ulong)StaticEnums.CardFlag.ATTACK) != 0)
                            {
                                cards.Add(card);
                            }
                        }
                        bs.DisCard(cards, pr, BattleScene.DisCardReason.EFFECT);
                        pr.CurrPBlock += cards.Count * 3;
                        pr.CurrMBlock += cards.Count * 3;
                    }
                }
                GD.Print($"卡牌C{c.Id}已使用");
            });
        }
    }
}
